using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using ExoMail.Smtp.IO;

namespace ExoMail.Smtp.Network
{
    public class SmtpServer
    {
        public IServerConfig ServerConfig { get; set; }
        public TcpListener TcpListener { get; set; }
        public List<SmtpSession> SmtpSessions { get; set; }
        public IMessageStore MessageStore { get; set; }
        public List<IUserAuthenticator> UserAuthenticators { get; set; }

        private CancellationToken Token { get; set; }

        public SmtpServer()
        {
            this.SmtpSessions = new List<SmtpSession>();
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
                tcpClient = await this.TcpListener.AcceptTcpClientAsync();
                StartNewSession(tcpClient);
            }
        }

        /// <summary>
        /// Stops this SmtpServer.Core.
        /// </summary>
        public void Stop()
        {
            this.TcpListener.Stop();
        }

        public void StartNewSession(TcpClient tcpClient)
        {
            Task.Run(async () =>
            {

                SmtpSession session = new SmtpSession(tcpClient)
                {
                    SmtpServer = this,
                    MessageStore = this.MessageStore,
                    UserAuthenticators = this.UserAuthenticators
                };

                await session.BeginSessionAsync();
            });
        }
    }
}