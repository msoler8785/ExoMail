using ExoMail.Smtp.Client.Authentication;
using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Extensions;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Protocol;
using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.Common.Utilities;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Network
{
    public class SmtpClient : IDisposable
    {
        public const int DEFAULT_PORT = 25;
        private const string NEWLINE = "\r\n";
        private static readonly string[] _seperator = new string[] { NEWLINE };
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private object _lock = new object();
        private string _password;
        private string _serverGreeting;
        private SslStream _sslStream;
        private TcpClient _tcpClient;
        private string _userName;

        public delegate void SessionQuitHandler(object smtpClient, EventArgs eventArgs);

        public event SessionQuitHandler SessionQuitting;

        public ClientState ClientState { get; set; } = ClientState.Pending;
        public string HostName { get; set; }
        public bool IsAuthenticated { get; private set; }
        public bool IsConnected => !IsDisposed && _tcpClient.Connected && ClientState == ClientState.Connected;
        public bool IsDisposed { get; private set; } = false;
        public bool IsEncrypted => _sslStream != null && _sslStream.IsEncrypted;
        public int Port { get; private set; }
        public IPAddress RcptHostAddress { get; set; }
        public ServerExtensions ServerExtensions { get; set; }
        internal Timer Timer { get; set; }
        private NetworkStream NetworkStream => _tcpClient.GetStream();

        private Stream Stream
        {
            get
            {
                if (IsEncrypted)
                    return _sslStream;
                else
                    return NetworkStream;
            }
        }

        public SmtpClient(IPAddress rcptHost = null, string hostName = null, int port = DEFAULT_PORT, string userName = null, string password = null, ILoggerFactory loggerFactory = null)
        {
            _tcpClient = new TcpClient();
            _userName = userName;
            _password = password;
            RcptHostAddress = rcptHost;
            HostName = hostName ?? Environment.MachineName;
            Port = port;
            ServerExtensions = ServerExtensions.EHLO;
            _loggerFactory = loggerFactory ?? new BasicConsoleLoggerFactory();
            _logger = _loggerFactory.CreateLogger<SmtpClient>();
        }

        public async Task ConnectAsync(string userName, string password)
        {
            _userName = userName;
            _password = password;

            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            ClientState = ClientState.Connecting;
            Timer = new Timer(TimeoutElapsed, this, 120000, -1);

            var saslInit = new SaslInitializer(_userName, _password, this);

            try
            {
                _logger.LogInformation("SmtpClient is connecting to {0}:{1}", RcptHostAddress, Port);

                await _tcpClient.ConnectAsync(RcptHostAddress, Port);
                _serverGreeting = await ListenResponseAsync();

                if (!_serverGreeting.StartsWith("220"))
                    throw new Exception(_serverGreeting);

                var initiation = new SessionInitiation(this);
                IExecutionResult result = await initiation.InvokeCommands();

                ParseServerExtensions(result.Response);

                if (ServerExtensions.HasExtension(ServerExtensions.STARTTLS))
                {
                    _logger.LogInformation("Negotiating TLS connection.");
                    var command = new SmtpStartTlsCommand(this);
                    await command.Invoke();
                    await command.RetrieveResult();
                    result = command.Result;

                    if (command.Result.IsSuccessfull)
                    {
                        // TODO: Make this configurable.
                        _sslStream = new SslStream(
                            NetworkStream,
                            false,
                            new RemoteCertificateValidationCallback((a, b, c, d) => true));

                        await _sslStream.AuthenticateAsClientAsync(HostName);
                        result = await initiation.InvokeCommands();
                        ParseServerExtensions(result.Response);
                    }
                }

                if (ServerExtensions.HasExtension(ServerExtensions.AUTH))
                {
                    var mechanisms = saslInit.GetMechanisms(result.Response);

                    foreach (var item in mechanisms)
                    {
                        var authCommand = new SmtpAuthCommand(this, item);

                        await authCommand.Invoke();
                        await authCommand.RetrieveResult();

                        if (authCommand.Result.IsSuccessfull)
                        {
                            result = await initiation.InvokeCommands();
                            ParseServerExtensions(result.Response);
                            break;
                        }
                    }
                }

                if (!result.IsSuccessfull)
                    throw new Exception(result.Response);

                ClientState = ClientState.Connected;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.HResult, ex, ex.Message);
                throw ex;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (!IsDisposed)
                {
                    _logger.LogInformation("SmtpClient session Disposing.");

                    try
                    {
                        QuitSession().Wait(15000);
                    }
                    catch (AggregateException ex)
                    {
                        _logger.LogWarning(ex.HResult, ex, ex.Message);
                    }
                    finally
                    {
                        _tcpClient.Dispose();
                        ClientState = ClientState.Disconnected;
                        IsDisposed = true;
                    }
                }
            }
        }

        /// <summary>
        /// Listen for the servers response.
        /// </summary>
        /// <returns>A string representing the servers response.</returns>
        public async Task<string> ListenResponseAsync()
        {
            Timer = new Timer(TimeoutElapsed, this, 60000, -1);

            //byte[] responseBytes = await Stream.ReadLine();
            string line = await Stream.ReadLine();
            string response = line;

            while (response.Length >= 4 && line[3] == '-')
            {
                line = await Stream.ReadLine();
                response += line;
            }

            _logger.LogInformation("Server: {0}", response.Replace(NEWLINE, "<CRLF>"));

            return response;
        }

        public async Task<bool> SendAsync(string sender, IEnumerable<string> recipients, string messagePath)
        {
            var envelopes = recipients.Select(x => new BasicEnvelope()
            {
                Sender = MailAddress.Parse(sender),
                Recipient = MailAddress.Parse(x)
            });

            using (var messageStream = new FileStream(messagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await SendAsync(sender, envelopes, messageStream);
            }
        }

        public async Task<bool> SendAsync(string sender, IEnumerable<IMessageEnvelope> envelopes, Stream messageStream, IDkimPolicy dkimPolicy = null)
        {
            Timer.Change(60000, 0);

            var message = MimeMessage.Load(messageStream);

            if (!ServerExtensions.HasExtension(ServerExtensions.EIGHTBITMIME))
            {
                _logger.LogInformation("Server does not support 8BITMIME extension. Converting to 7bit content.");
                message.Prepare(EncodingConstraint.SevenBit);
            }

            if (dkimPolicy != null)
            {
                _logger.LogInformation("DKIM singing message.");
                message.Sign(dkimPolicy.DkimSigner, dkimPolicy.Headers);
            }

            long size = messageStream.Length;
            List<ISmtpCommand> commands = new List<ISmtpCommand>();
            commands.Add(new SmtpMailCommand(this, sender, size));
            var rcptCommands = envelopes.Select(envelope => new SmtpRcptCommand(this, envelope)).ToList();
            commands.AddRange(rcptCommands);
            var dataCommand = new SmtpDataCommand(this, message, Stream, _loggerFactory);
            commands.Add(dataCommand);

            if (ServerExtensions.HasExtension(ServerExtensions.PIPELINING))
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    await commands[i].Invoke();
                }

                for (int i = 0; i < commands.Count; i++)
                {
                    await commands[i].RetrieveResult();
                }
            }
            else
            {
                foreach (var command in commands)
                {
                    await command.Invoke();
                    await command.RetrieveResult();
                }
            }

            if (dataCommand.Result.IsSuccessfull)
            {
                DateTime sentDate = DateTime.Now;

                foreach (var envelope in envelopes)
                {
                    if (envelope.Disposition.HasDisposition(Disposition.Delivered))
                    {
                        envelope.LastResponse = dataCommand.Result.Response;
                        envelope.SentDate = sentDate;
                    }
                }
            }
            else
            {
                await ResetSession();
            }

            return dataCommand.Result.IsSuccessfull;
        }

        /// <summary>
        /// Sends a command to the server.
        /// </summary>
        /// <param name="command">A string representing the command to send to the server.</param>
        /// <returns>Task</returns>
        public async Task SendCommandAsync(string command)
        {
            byte[] commandBytes = Encoding.ASCII.GetBytes(command + NEWLINE);

            await Stream.WriteAsync(commandBytes, 0, commandBytes.Length);
            _logger.LogInformation("Client: {0}", command);
        }

        protected void OnSessionQuitting(object smtpClient, EventArgs eventArgs)
        {
            SessionQuitting?.Invoke(smtpClient, eventArgs);
        }

        private void ParseServerExtensions(string response)
        {
            _logger.LogDebug("Parsing server extensions.");
            response = response.ToUpper();
            ServerExtensions = ServerExtensions.EHLO;

            if (response.Contains("STARTTLS"))
                ServerExtensions |= ServerExtensions.STARTTLS;
            if (response.Contains("SIZE"))
                ServerExtensions |= ServerExtensions.SIZE;
            if (response.Contains("PIPELINING"))
                ServerExtensions |= ServerExtensions.PIPELINING;
            if (response.Contains("AUTH"))
                ServerExtensions |= ServerExtensions.AUTH;
            if (response.Contains("ENHANCEDSTATUSCODES"))
                ServerExtensions |= ServerExtensions.ENHANCEDSTATUSCODES;
            if (response.Contains("DSN"))
                ServerExtensions |= ServerExtensions.DSN;
            if (response.Contains("8BITMIME"))
                ServerExtensions |= ServerExtensions.EIGHTBITMIME;
            if (response.Contains("SMTPUTF8"))
                ServerExtensions |= ServerExtensions.SMTPUTF8;
        }

        private async Task QuitSession()
        {
            _logger.LogInformation("SmtpClient session quiting.");
            OnSessionQuitting(this, new EventArgs());

            if (IsConnected)
            {
                ClientState = ClientState.Disconnecting;
                var quit = new SmtpQuitCommand(this);
                await quit.Invoke();
                await quit.RetrieveResult();
            }
        }

        private async Task ResetSession()
        {
            _logger.LogInformation("SmtpClient Session Resetting.");

            var rset = new SmtpRsetCommand(this);
            await rset.Invoke();
            await rset.RetrieveResult();

            if (!rset.Result.IsSuccessfull)
                await QuitSession();
        }

        private void TimeoutElapsed(object state)
        {
            Dispose();
        }
    }
}