using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Protocol;
using ExoMail.Smtp.Server.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Network
{
    public class SmtpServer : ISmtpServer
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMessageStore _messageStore;
        private readonly IVirtualServerRepository _virtualServerRepository;
        private ISmtpServerConfig _serverConfig;
        private TcpListener _tcpListener;
        public bool IsRunning { get; private set; }

        public SmtpServer(IMessageStore messageStore, ILoggerFactory loggerFactory, IVirtualServerRepository virtualServerRepository)
        {
            _messageStore = messageStore;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SmtpServer>();
            _virtualServerRepository = virtualServerRepository;
        }

        public async Task StartAsync(ISmtpServerConfig serverConfig, CancellationToken token)
        {
            _serverConfig = serverConfig;
            _logger.LogInformation("Starting up SmtpListener on {0}:{1}", _serverConfig.ServerIpBinding, _serverConfig.Port);
            _tcpListener = new TcpListener(_serverConfig.ServerIpBinding, _serverConfig.Port);
            token.Register(() => Shutdown(_tcpListener));
            _tcpListener.Start();

            IsRunning = true;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient smtpClient = await _tcpListener
                        .AcceptTcpClientAsync(token);
                    StartSessionAsync(smtpClient).RunAsync();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("SmtpServer task is canceled. Listener is shutting down.");
                }
            }
            IsRunning = false;

        }

        private async Task StartSessionAsync(TcpClient tcpClient)
        {
            _logger.LogInformation("System: Accepted new SMTP connection from {0}", tcpClient.Client.RemoteEndPoint.ToString());

            var session = new SmtpSession(_loggerFactory, _virtualServerRepository, _messageStore, _serverConfig);

            try
            {
                SessionManager.GetSessionManager.SmtpSessions.Add(session);
                await session.BeginSession(tcpClient);
            }
            finally
            {
                SessionManager.GetSessionManager.SmtpSessions.Remove(session);
            }
        }

        public void Shutdown(TcpListener tcpListener)
        {
            tcpListener.Stop();

            var sessions = SessionManager.GetSessionManager.SmtpSessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                sessions[i].StopSession();
            }
        }
    }
}