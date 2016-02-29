using ARSoft.Tools.Net;
using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Models;
using ExoMail.Smtp.Utilities;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ExoMail.Smtp.Network
{
    /// <summary>
    /// Base class of an SmtpSession.
    /// </summary>
    public abstract class SmtpSessionBase
    {
        public const string NEWLINE = "\r\n";
        public const int ONE_MB = 1048576;
        public const string TERMINATOR = "\r\n.\r\n";
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public bool IsAuthenticated { get; set; }
        public bool IsAuthenticatedRelayAllowed { get; set; }

        public bool IsEncrypted
        {
            get
            {
                if (this.SslStream != null)
                    return this.SslStream.IsEncrypted;
                else
                    return false;
            }
        }

        public IPEndPoint LocalEndPoint { get { return (IPEndPoint)this.TcpClient.Client.LocalEndPoint; } }
        public IMessageStore MessageStore { get; set; }
        public NetworkStream NetworkStream { get { return this.TcpClient.GetStream(); } }
        public StreamReader Reader { get; set; }
        public IPEndPoint RemoteEndPoint { get { return (IPEndPoint)this.TcpClient.Client.RemoteEndPoint; } }
        public List<SmtpCommand> SmtpCommands { get; set; }
        public SmtpServer SmtpServer { get; set; }
        public SslStream SslStream { get; set; }
        public TcpClient TcpClient { get; set; }
        public System.Timers.Timer Timer { get; set; }
        public CancellationToken Token { get; set; }
        public StreamWriter Writer { get; set; }
        public DomainName RemoteDomainName { get; set; }
        public List<ISaslAuthenticator> UserAuthenticators { get; set; }
        public List<string> SaslMechanisms
        {
            get
            {
                return this.UserAuthenticators.Select(x => x.SaslMechanism).ToList();
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SmtpSessionBase() { }

        /// <summary>
        /// Constructor that initializes the minimum amount of properties for a session.
        /// </summary>
        /// <param name="tcpClient">A tcpclient to initiate the session with.</param>
        public SmtpSessionBase(TcpClient tcpClient)
            : this(tcpClient, CancellationToken.None)
        { }

        /// <summary>
        /// Constructor that initializes the minimum amount of properties for a session
        /// and a cancellation token.
        /// </summary>
        /// <param name="tcpClient">A tcpclient to initiate the session with.</param>
        /// <param name="cancellationToken">A CancellationToken instance to cancel the session.</param>
        public SmtpSessionBase(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            if (cancellationToken == CancellationToken.None)
            {
                this.CancellationTokenSource = new CancellationTokenSource();
                this.Token = this.CancellationTokenSource.Token;
            }
            else
            {
                this.Token = cancellationToken;
            }
            this.Token.Register(() => StopSession());
            this.TcpClient = tcpClient;
            this.SmtpCommands = new List<SmtpCommand>();
        }

        public abstract Task BeginSessionAsync();

        /// <summary>
        /// Gets the sessions response in reply to the clients SmtpCommand.
        /// </summary>
        /// <param name="smtpCommand">A SmtpCommand instance parsed from the client's request.</param>
        /// <returns>A string indicating the servers response to the requested command.</returns>
        protected async Task<string> GetResponseAsync(SmtpCommand smtpCommand)
        {
            string response;

            switch (smtpCommand.CommandType)
            {
                case SmtpCommandType.EHLO:
                    response = GetEhloResponse(smtpCommand);
                    break;

                case SmtpCommandType.HELO:
                    response = GetHeloResponse(smtpCommand);
                    break;

                case SmtpCommandType.AUTH:
                    response = await GetAuthResponse(smtpCommand);
                    break;

                case SmtpCommandType.MAIL:
                    response = GetMailResponse(smtpCommand);
                    break;

                case SmtpCommandType.RCPT:
                    response = GetRcptResponse(smtpCommand);
                    break;

                case SmtpCommandType.DATA:
                    response = await GetDataResponse(smtpCommand);
                    break;

                case SmtpCommandType.RSET:
                    response = SmtpResponse.OK;
                    Reset();
                    break;

                case SmtpCommandType.QUIT:
                    response = String.Format(SmtpResponse.Closing, this.SmtpServer.ServerConfig.HostName);
                    this.SmtpCommands.Add(smtpCommand);
                    break;

                case SmtpCommandType.NOOP:
                    response = SmtpResponse.OK;
                    break;

                case SmtpCommandType.STARTTLS:
                    if (smtpCommand.Arguments.Any())
                    {
                        response = SmtpResponse.ArgumentUnrecognized;
                    }
                    else
                    {
                        response = SmtpResponse.StartTls;
                    }
                    break;

                case SmtpCommandType.HELP:
                    response = SmtpResponse.Help;
                    break;

                case SmtpCommandType.SAML:
                    response = SmtpResponse.CommandNotImplemented;
                    break;

                case SmtpCommandType.VRFY:
                    response = SmtpResponse.CommandNotImplemented;
                    break;

                case SmtpCommandType.TURN:
                    response = SmtpResponse.CommandNotImplemented;
                    break;

                default:
                    response = SmtpResponse.CommandUnrecognized;
                    break;
            }
            return response;
        }

        private async Task<string> GetAuthResponse(SmtpCommand smtpCommand)
        {
            string response;
            if (!this.SmtpCommands.Any(c => c.Command == "EHLO" || c.Command == "HELO"))
            {
                response = SmtpResponse.BadCommand;
            }
            else
            {
                response = await AuthenticateUser(smtpCommand);
            }

            return response;
        }

        private async Task<string> AuthenticateUser(SmtpCommand smtpCommand)
        {
            //Get the authentication mechanism requested from the client.
            var saslMechanism = smtpCommand.Arguments.ElementAtOrDefault(0);

            //Select the authenticator that matches the requested mechanism.
            ISaslAuthenticator authenticator = this.UserAuthenticators
                .Where(x => x.SaslMechanism == saslMechanism.ToUpper())
                .FirstOrDefault()
                .Create();

            //If there are no authenticators for the requested type return ArgumentUnrecognized to the client.
            if (authenticator == null)
            {
                return SmtpResponse.ArgumentUnrecognized;
            }

            if (authenticator.IsInitiator)
            {
                try
                {
                    while (!authenticator.IsCompleted)
                    {
                        await SendResponseAsync(authenticator.GetChallenge());
                        authenticator.ParseResponse(await ListenRequestAsync());
                    }
                }
                catch (SaslException ex)
                {
                    return SmtpResponse.AuthCredInvalid + ex.Message;
                }
            }
            else
            {
                // TODO: Some SASL mechanisms are initiated by the client.  When we implement
                // those mechanism we will handle the logic here.
                return SmtpResponse.ArgumentUnrecognized;
            }

            if (authenticator.IsAuthenticated)
            {
                return SmtpResponse.AuthOk;
            }
            else
            {
                return SmtpResponse.AuthCredInvalid;
            }
        }

        /// <summary>
        /// Callback method for the session Timer timeout.
        /// </summary>
        /// <param name="obj"></param>
        protected void IdleTimeout(Object obj, ElapsedEventArgs args)
        {
            try
            {
                SendResponseAsync(String.Format(SmtpResponse.Closing, this.SmtpServer.ServerConfig.HostName)).Wait(TimeSpan.FromSeconds(5));
                this.CancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on idle timeout. " + ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Initialize the network stream depending on whether the session
        /// is configured to use Tls or not.
        /// </summary>
        /// <returns>Task</returns>
        protected async Task InitializeStreams()
        {
            if (this.SmtpServer.ServerConfig.IsTls)
            {
                await StartTlsAsync();
            }
            else
            {
                this.Writer = new StreamWriter(this.NetworkStream) { AutoFlush = true, NewLine = NEWLINE };
                this.Reader = new StreamReader(this.NetworkStream);
            }
        }

        protected bool IsValidatedSession()
        {
            if (this.SmtpServer.ServerConfig.SessionValidators != null)
                return this.SmtpServer.ServerConfig.SessionValidators.All(x => x.IsValid());
            else
                return true;
        }

        /// <summary>
        /// Listen for the clients request.
        /// </summary>
        /// <returns>A string representing the clients request.</returns>
        protected async Task<string> ListenRequestAsync()
        {
            string response = String.Empty;

            while (String.IsNullOrEmpty(response))
            {
                response = await this.Reader.ReadLineAsync().WithCancellation(this.Token);
            }
            Console.WriteLine("Client <<<: {0}", response);

            return response;
        }

        /// <summary>
        /// Sends a response to the client.
        /// </summary>
        /// <param name="response">A string representing the response to the client.</param>
        /// <returns>Task</returns>
        protected async Task SendResponseAsync(string response)
        {
            Console.WriteLine("Server >>>: {0}", response);
            await this.Writer.WriteLineAsync(response);
        }

        /// <summary>
        /// Initiates the StartTls command negotiation.
        /// </summary>
        /// <returns>Task</returns>
        protected async Task StartTlsAsync()
        {
            this.SslStream = new SslStream(this.NetworkStream, false);
            var protocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            await this.SslStream.AuthenticateAsServerAsync(this.SmtpServer.ServerConfig.X509Certificate2, false, protocols, false);

            this.Reader = new StreamReader(this.SslStream);
            this.Writer = new StreamWriter(this.SslStream) { AutoFlush = true, NewLine = "\r\n" };
            this.SmtpCommands.Clear();
        }

        public abstract void StopSession();

        /// <summary>
        /// Response logic if STARTTLS is required.
        /// </summary>
        /// <param name="smtpCommand">A client SmtpCommand to respond to.</param>
        /// <returns>string representing the servers response.</returns>
        protected string WaitingForTls(SmtpCommand smtpCommand)
        {
            string response;

            switch (smtpCommand.CommandType)
            {
                case SmtpCommandType.EHLO:
                    response = GetEhloResponse(smtpCommand);
                    break;

                case SmtpCommandType.QUIT:
                    response = String.Format(SmtpResponse.Closing, this.SmtpServer.ServerConfig.HostName);
                    this.SmtpCommands.Add(smtpCommand);
                    break;

                case SmtpCommandType.NOOP:
                    response = SmtpResponse.OK;
                    break;

                case SmtpCommandType.STARTTLS:
                    response = SmtpResponse.StartTls;
                    break;

                case SmtpCommandType.INVALID:
                    response = SmtpResponse.CommandUnrecognized;
                    break;

                default:
                    if (Enum.IsDefined(typeof(SmtpCommandType), smtpCommand.CommandType))
                        response = SmtpResponse.StartTlsFirst;
                    else
                    {
                        response = SmtpResponse.CommandUnrecognized;
                    }
                    break;
            }
            return response;
        }

        private async Task<string> GetDataResponse(SmtpCommand smtpCommand)
        {
            string response;
            if (!this.SmtpCommands.Any(c => c.Command == "EHLO" || c.Command == "HELO"))
            {
                response = SmtpResponse.BadCommand;
            }
            else if (!this.SmtpCommands.Any(c => c.Command == "RCPT"))
            {
                response = SmtpResponse.SenderAndRecipientFirst;
            }
            else if (!this.SmtpCommands.Any(c => c.Command == "MAIL"))
            {
                response = SmtpResponse.SenderFirst;
            }
            else
            {
                this.SmtpCommands.Add(smtpCommand);
                response = await StartInputAsync();
                Reset();
            }
            return response;
        }

        private string GetEhloResponse(SmtpCommand smtpCommand)
        {
            if (smtpCommand.Arguments.Count > 1)
            {
                return SmtpResponse.ArgumentUnrecognized;
            }
            string response;
            DomainName domainName;
            bool isValidDomain = DomainName.TryParse(smtpCommand.Arguments[0], out domainName);
            var config = this.SmtpServer.ServerConfig;

            if (isValidDomain)
            {
                if (smtpCommand.Command == "EHLO")
                {
                    string saslMechanisms = string.Join(" ", this.SaslMechanisms);
                    response = String.Format(SmtpResponse.Ehlo, config.HostName, domainName.ToString(), config.MaxMessageSize.ToString(), saslMechanisms);
                }
                else
                {
                    response = String.Format(SmtpResponse.Hello, config.HostName, domainName.ToString());
                }
                this.RemoteDomainName = domainName;
                this.SmtpCommands.Clear();
                this.SmtpCommands.Add(smtpCommand);
            }
            else
            {
                response = SmtpResponse.InvalidDomainName;
            }

            return response;
        }

        private string GetHeloResponse(SmtpCommand smtpCommand)
        {
            return GetEhloResponse(smtpCommand);
        }

        private string GetMailResponse(SmtpCommand smtpCommand)
        {
            string response;
            if (!this.SmtpCommands.Any(c => c.Command == "EHLO" || c.Command == "HELO"))
            {
                response = SmtpResponse.BadCommand;
            }
            else if (!smtpCommand.Arguments[0].Contains("FROM:"))
            {
                response = SmtpResponse.InvalidSenderName;
            }
            else
            {
                this.SmtpCommands.Add(smtpCommand);
                response = SmtpResponse.OK;
            }
            return response;
        }

        private string GetRcptResponse(SmtpCommand smtpCommand)
        {
            string response;
            if (!this.SmtpCommands.Any(c => c.Command == "EHLO" || c.Command == "HELO"))
            {
                response = SmtpResponse.BadCommand;
            }
            else if (!this.SmtpCommands.Any(c => c.Command == "MAIL"))
            {
                response = SmtpResponse.SenderFirst;
            }
            else if (!smtpCommand.Arguments[0].Contains("TO:"))
            {
                response = SmtpResponse.InvalidRecipient;
            }
            else
            {
                this.SmtpCommands.Add(smtpCommand);
                response = SmtpResponse.OK;
            }
            return response;
        }

        /// <summary>
        /// Receives the data portion of the SMTP transaction.
        /// </summary>
        /// <param name="stream">A NetworkStream or SslStream to read the data from.</param>
        /// <returns>A string indicating the result of the transaction.</returns>
        private async Task<string> ReceiveDataAsync(Stream stream)
        {
            using (var memoryStream = new RecyclableMemoryStreamManager().GetStream())
            using (var reader = new StreamReader(memoryStream, Encoding.ASCII))
            {
                // 8KB buffer for NetworkStream.
                byte[] buffer = new byte[8 * 1024];

                // Number of bytes read from NetworkStream.
                int bytesRead;

                while (!reader.ReadToEnd().Contains(TERMINATOR))
                {
                    // Read the NetworkStream.
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, this.Token);

                    // Write stream to MemoryStream for replay.
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);

                    // Rewind 8 bytes so that we can check for line terminator.
                    if (memoryStream.Length >= 8)
                        memoryStream.Seek(-8, SeekOrigin.End);
                }

                if (memoryStream.Length > this.SmtpServer.ServerConfig.MaxMessageSize)
                {
                    Reset();
                    double maxMessageSize = this.SmtpServer.ServerConfig.MaxMessageSize / ONE_MB;

                    Console.WriteLine("System >>>: Received: {0} bytes.  MaxMessageSize: {1} bytes.  Over by {2} bytes.",
                        memoryStream.Length, this.SmtpServer.ServerConfig.MaxMessageSize, memoryStream.Length - this.SmtpServer.ServerConfig.MaxMessageSize);
                    return String.Format(SmtpResponse.MessageSizeExceeded, maxMessageSize.ToString());
                }

                memoryStream.Position = 0;
                Console.WriteLine("System >>>: Received {0} bytes from {1}:{2}", memoryStream.Length, this.RemoteEndPoint.Address, this.RemoteEndPoint.Port);

                ReceivedHeader sessionMessage = new ReceivedHeader()
                {
                    ClientHostName = this.RemoteDomainName.ToString(),
                    IsEncrypted = this.IsEncrypted,
                    LocalEndPoint = this.LocalEndPoint,
                    RemoteEndPoint = this.RemoteEndPoint,
                    ServerHostName = this.SmtpServer.ServerConfig.HostName,
                };

                this.MessageStore.Save(memoryStream, sessionMessage);
            }

            var recipientCollections = new MailRecipientCollection().GetRecipientCollections(this.SmtpCommands);

            return SmtpResponse.Queued;
        }

        /// <summary>
        /// Resets the session.
        /// </summary>
        private void Reset()
        {
            this.Reader.DiscardBufferedData();
            this.Writer.Flush();
            this.SmtpCommands = this.SmtpCommands.Where(x => x.Command == "HELO" || x.Command == "EHLO").ToList();
        }

        /// <summary>
        /// Enter into the data phase of the SmtpSession
        /// </summary>
        /// <returns>SmtpReply.Queued response code.</returns>
        private async Task<string> StartInputAsync()
        {
            await SendResponseAsync(SmtpResponse.StartInput);
            string response;

            if (this.IsEncrypted)
                response = await ReceiveDataAsync(this.SslStream);
            else
                response = await ReceiveDataAsync(this.NetworkStream);

            return response;
        }
    }
}