using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Extensions;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Protocol;
using ExoMail.Smtp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Network
{
    public class SmtpServer
    {
        private IServerConfig _serverConfig { get; set; }
        private IMessageStore _messageStore { get; set; }
        private TcpListener _tcpListener { get; set; }
        private CancellationToken _token { get; set; }
        private List<ISaslMechanism> _saslMechanisms { get; set; }

        public SmtpServer(IServerConfig serverConfig, IMessageStore messageStore)
        {
            this._serverConfig = serverConfig;
            this._messageStore = messageStore;
            this._saslMechanisms = new List<ISaslMechanism>();
        }

        public async Task StartAsync(CancellationToken token)
        {
            this._token = token;
            this._tcpListener = new TcpListener(this._serverConfig.ServerIpBinding, this._serverConfig.Port);
            this._tcpListener.Start();
            this._saslMechanisms.Add(new LoginSaslMechanism());
            this._saslMechanisms.Add(new PlainSaslMechanism());
            TcpClient tcpClient;

            while (!this._token.IsCancellationRequested)
            {
                tcpClient = await this._tcpListener.AcceptTcpClientAsync().WithCancellation(this._token);
                CreateSession(tcpClient);
            }
            SessionManager.GetSessionManager.StopSessions();
        }

        private void CreateSession(TcpClient tcpClient)
        {
            Task.Run(async () =>
            {
                var session = new SmtpSession()
                {
                    ServerConfig = this._serverConfig,
                    MessageStore = this._messageStore,
                    SaslMechanisms = this._saslMechanisms
                };

                try
                {
                    SessionManager.GetSessionManager.SmtpSessions.Add(session);
                    await session.BeginSession(tcpClient);
                }
                finally
                {
                    SessionManager.GetSessionManager.SmtpSessions.Remove(session);
                }
            });
        }
    }
}