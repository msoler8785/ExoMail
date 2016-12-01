using ExoMail.Smtp.Extensions;
using ExoMail.Smtp.Server.Extensions;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Protocol;
using ExoMail.Smtp.Server.Services;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Network
{
    public class SmtpServer
    {
        private IServerConfig _serverConfig { get; set; }
        private IMessageStore _messageStore { get; set; }
        private TcpListener _tcpListener { get; set; }
        private CancellationToken _token { get; set; }

        public SmtpServer(IServerConfig serverConfig, IMessageStore messageStore)
        {
            this._serverConfig = serverConfig;
            this._messageStore = messageStore;
        }

        public async Task StartAsync(CancellationToken token)
        {
            this._token = token;
            this._tcpListener = new TcpListener(this._serverConfig.ServerIpBinding, this._serverConfig.Port);
            this._tcpListener.Start();

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