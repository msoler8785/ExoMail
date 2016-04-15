using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Network
{
    public class SmtpSessionFactory
    {
        public static List<IAuthorizedDomain> AuthorizedDomains { get; set; }
        public static List<SmtpSessionBase> SmtpSessions { get; set; }

        public IMessageStore MessageStore { get; set; }
        public IServerConfig ServerConfig { get; set; }
        public TcpListener TcpListener { get; set; }
        public List<ISaslAuthenticator> UserAuthenticators { get; set; }
        public IUserStore UserStore { get; set; }
        private CancellationToken Token { get; set; }

        public SmtpSessionFactory()
        {
            SmtpSessions = new List<SmtpSessionBase>();
            this.UserAuthenticators = new List<ISaslAuthenticator>();
        }

        public void CreateSession(TcpClient tcpClient)
        {
            Task.Run(async () =>
            {
                var session = GetSessionType(tcpClient);
                session.MessageStore = this.MessageStore;
                session.UserAuthenticators = this.UserAuthenticators;
                session.ServerConfig = this.ServerConfig;
                session.AuthorizedDomains = AuthorizedDomains;
                session.UserStore = this.UserStore;

                SmtpSessions.Add(session);

                await session.BeginSessionAsync();

                SmtpSessions.Remove(session);
            });
        }

        /// <summary>
        /// Starts this SmtpServer with no Cancellation token set.
        /// </summary>
        /// <returns>Task</returns>
        public async Task Start()
        {
            await Start(CancellationToken.None);
            return;
        }

        /// <summary>
        /// Starts the SmtpServer with the specified CancellationToken
        /// </summary>
        /// <param name="token">The CancellationToken to use for this server instance.</param>
        /// <returns>Task</returns>
        public async Task Start(CancellationToken token)
        {
            this.TcpListener = new TcpListener(this.ServerConfig.ServerIpBinding, this.ServerConfig.Port);
            this.Token = token;
            this.TcpListener.Start();
            TcpClient tcpClient;

            while (!token.IsCancellationRequested)
            {
                tcpClient = await this.TcpListener.AcceptTcpClientAsync().WithCancellation(token);
                if (!token.IsCancellationRequested)
                {
                    CreateSession(tcpClient);
                }
            }
        }
        public void Stop()
        {
            this.TcpListener.Stop();
        }

        private SmtpSessionBase GetSessionType(TcpClient tcpClient)
        {
            SmtpSessionBase session;

            switch (this.ServerConfig.ServerType)
            {
                case ServerType.Originator:
                    session = null;
                    break;

                case ServerType.Delivery:
                    session = new SmtpDeliverySession(tcpClient, this.Token);
                    break;

                case ServerType.Relay:
                    session = new SmtpRelaySession(tcpClient, this.Token);
                    break;

                case ServerType.Gateway:
                    session = null;
                    break;

                default:
                    session = null;
                    break;
            }
            return session;
        }
    }
}