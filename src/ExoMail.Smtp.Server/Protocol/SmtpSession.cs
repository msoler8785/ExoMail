using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Utilities;
using ExoMail.Smtp.Server.Authentication;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpSession
    {
        /// <summary>
        /// The line termination sequence.
        /// <see cref="https://tools.ietf.org/html/rfc2821#section-2.3.7"/>
        /// </summary>
        public const string NEWLINE = "\r\n";

        public readonly IMessageStore _messageStore;
        public readonly ISmtpServerConfig _serverConfig;
        internal readonly IVirtualServerRepository _virtualServerRepository;
        private static readonly string[] _seperator = new string[] { NEWLINE };
        private static readonly byte[] NEWLINE_BYTES = Encoding.ASCII.GetBytes(NEWLINE);
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        public bool IsAuthenticated { get; internal set; }
        public SaslFactory SaslFactory { get; set; }
        public string SessionId { get; set; }
        public SmtpSessionNetwork SessionNetwork { get; set; }
        public List<SmtpCommandBase> SmtpCommands { get; set; }
        public SmtpStateTable SmtpStateTable { get; set; }
        public CancellationToken Token { get; private set; }
        public SmtpCancellationTokenSource TokenSource { get; private set; }

        public SmtpSession(
            ILoggerFactory loggerFactory,
            IVirtualServerRepository virtualServerRepository,
            IMessageStore messageStore,
            ISmtpServerConfig smtpServerConfig
            )
        {
            SessionId = DateTime.Now.Ticks.ToString("X");
            TokenSource = new SmtpCancellationTokenSource();
            Token = TokenSource.Token;
            SmtpCommands = new List<SmtpCommandBase>();
            SmtpStateTable = new SmtpStateTable();
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SmtpSession>();
            _virtualServerRepository = virtualServerRepository;
            _messageStore = messageStore;
            _serverConfig = smtpServerConfig;
            SaslFactory = new SaslFactory(virtualServerRepository);
        }

        public async Task BeginSession(TcpClient tcpClient)
        {
            await InitializeStreams(tcpClient);
            var sessionTimer = new SessionTimer(TimeSpan.FromMilliseconds(_serverConfig.SessionTimeout));
            sessionTimer.TimeoutElapsed += TimeoutElapsed;
            sessionTimer.Start();

            try
            {
                var commandFactory = new SmtpCommandFactory(this, _loggerFactory);

                _logger.LogInformation("Session: {0} System: Beginning session with {1}:{2}", SessionId, SessionNetwork.RemoteEndPoint.Address, SessionNetwork.RemoteEndPoint.Port);
                await SendResponseAsync(String.Format(SmtpResponse.Announcment, this._serverConfig.HostName));

                while (!this.Token.IsCancellationRequested)
                {
                    var requests = await ListenRequestAsync().WithCancellation(this.Token);

                    // Reset the idle timer.
                    sessionTimer.Restart();

                    var commands = requests.Select(command => commandFactory.Parse(command));

                    foreach (var command in commands)
                    {
                        await command.ExecuteAsync();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                string response = TokenSource.CancellationReason ?? SmtpResponse.LocalError;
                await SendResponseAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.HResult, ex, ex.Message);
                await SendResponseAsync(SmtpResponse.LocalError);
            }
            finally
            {
                _logger.LogInformation("Session: {0} System: closing streams", SessionId);
                this.SessionNetwork.Stream.Dispose();
                this.SessionNetwork.TcpClient.Close();
            }
        }

        /// <summary>
        /// Listen for the clients request.
        /// </summary>
        /// <returns>A string representing the clients request.</returns>
        public async Task<List<string>> ListenRequestAsync()
        {
            var requests = new List<string>();

            do
            {
                string line = await SessionNetwork.Stream.ReadLine();
                requests.Add(line);
            }
            while (SessionNetwork.NetworkStream.DataAvailable);

            _logger.LogInformation("Session: {0} Client: {1}", SessionId, String.Join("<CRLF>", requests) + "<CRLF>");
            return requests;
        }

        public void Reset()
        {
            _logger.LogInformation("Session: {0} System: Session resetting.", SessionId);
            SmtpStateTable.Reset();
        }

        /// <summary>
        /// Sends a response to the client.
        /// </summary>
        /// <param name="response">A string representing the response to the client.</param>
        /// <returns>Task</returns>
        public async Task SendResponseAsync(string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response + NEWLINE);

            await this.SessionNetwork.Stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            _logger.LogInformation("Session: {0} Server: {1}", SessionId, response.Replace(NEWLINE, "<CRLF>"));
        }

        /// <summary>
        /// Initiates the StartTls command negotiation.
        /// </summary>
        /// <returns>Task</returns>
        public async Task StartTlsAsync()
        {
            _logger.LogInformation("Session: {0} System: Establishing TLS connection.", SessionId);
            this.SessionNetwork.SslStream = new SslStream(this.SessionNetwork.Stream, false);
            var protocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

            await this.SessionNetwork.SslStream.AuthenticateAsServerAsync(
                this._serverConfig.X509Certificate2, false, protocols, false);
        }

        public void StopSession()
        {
            _logger.LogInformation("Session: {0} System: Session stopping.", SessionId);
            TokenSource.Cancel(SmtpResponse.SystemShuttingDown);
        }

        /// <summary>
        /// Initialize the network stream depending on whether the session
        /// is configured to use Tls or not.
        /// </summary>
        /// <returns>Task</returns>
        protected async Task InitializeStreams(TcpClient tcpClient)
        {
            _logger.LogInformation("Session: {0} System: initializing streams", SessionId);

            this.SessionNetwork = new SmtpSessionNetwork(tcpClient);

            if (this._serverConfig.IsTls)
            {
                await StartTlsAsync();
            }
        }

        private void TimeoutElapsed(object sessionTimer, EventArgs args)
        {
            this.TokenSource.Cancel(SmtpResponse.Timeout);
        }
    }
}