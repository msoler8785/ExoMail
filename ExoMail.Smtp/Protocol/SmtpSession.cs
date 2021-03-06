﻿using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Extensions;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpSession
    {
        /// <summary>
        /// The line termination sequence.
        /// <see cref="https://tools.ietf.org/html/rfc2821#section-2.3.7"/>
        /// </summary>
        public const string NEWLINE = "\r\n";

        /// <summary>
        /// The number of bytes in one MB.
        /// </summary>
        public const int ONE_MB = 1048576;

        /// <summary>
        /// The data termination sequence.
        /// <see cref="https://tools.ietf.org/html/rfc2821#section-4.2.5"/>
        /// </summary>
        public const string TERMINATOR = "\r\n.\r\n";

        public SmtpSessionNetwork SessionNetwork { get; set; }
        public List<SmtpCommandBase> SmtpCommands { get; set; }
        public IServerConfig ServerConfig { get; set; }

        public bool IsEncrypted
        {
            get
            {
                bool isEncrypted =
                    this.SessionNetwork.SslStream == null ?
                    false :
                    this.SessionNetwork.SslStream.IsEncrypted;

                return isEncrypted;
            }
        }

        public StreamReader Reader { get; internal set; }
        public StreamWriter Writer { get; internal set; }

        /// <summary>
        /// Monitors the time between commands and disconnects the client
        /// if it is idle for too long.
        /// </summary>
        public System.Timers.Timer Timer { get; set; }

        public SessionState SessionState { get; set; }
        public CancellationTokenSource TokenSource { get; private set; }
        public CancellationToken Token { get; private set; }
        public IMessageStore MessageStore { get; set; }
        public bool IsAuthenticated { get; internal set; }

        public IMessageEnvelope MessageEnvelope { get; set; }

        public SmtpSession()
        {
            this.TokenSource = new CancellationTokenSource();
            this.Token = this.TokenSource.Token;
            this.SmtpCommands = new List<SmtpCommandBase>();
            this.SessionState = SessionState.EhloNeeded;
            this.MessageEnvelope = new MessageEnvelope();
        }

        /// <summary>
        /// Initialize the network stream depending on whether the session
        /// is configured to use Tls or not.
        /// </summary>
        /// <returns>Task</returns>
        protected async Task InitializeStreams(TcpClient tcpClient)
        {
            this.SessionNetwork = new SmtpSessionNetwork(tcpClient);

            if (this.ServerConfig.IsTls)
            {
                await StartTlsAsync();
            }
            else
            {
                this.Writer = new StreamWriter(this.SessionNetwork.NetworkStream, Encoding.UTF8)
                {
                    AutoFlush = true,
                    NewLine = NEWLINE
                };

                this.Reader = new StreamReader(this.SessionNetwork.NetworkStream, Encoding.UTF8);
            }
        }

        public async Task BeginSession(TcpClient tcpClient)
        {
            await InitializeStreams(tcpClient);
            this.Timer = new System.Timers.Timer((double)this.ServerConfig.SessionTimeout);
            this.Timer.Elapsed += IdleTimeout;
            this.Timer.AutoReset = false;
            this.Timer.Enabled = true;

            try
            {
                var commandFactory = new SmtpCommandFactory(this);

                await SendResponseAsync(String.Format(SmtpResponse.Announcment, this.ServerConfig.HostName));

                while (!this.SmtpCommands.Any(x => x.CommandType == SmtpCommandType.QUIT))
                {
                    this.Token.ThrowIfCancellationRequested();
                    var request = await ListenRequestAsync();

                    // Reset the idle timer.
                    this.Timer.Stop();
                    this.Timer.Start();

                    var smtpCommand = commandFactory.Parse(request);
                    var response = await smtpCommand.GetResponseAsync();

                    await SendResponseAsync(response);
                    await smtpCommand.ProcessCommandAction();
                }
            }
            catch (OperationCanceledException)
            {
                await SendResponseAsync(String.Format(SmtpResponse.Closing, this.ServerConfig.HostName));
            }
            catch (Exception)
            {
                await SendResponseAsync(SmtpResponse.LocalError);
            }
            finally
            {
                this.Reader.Dispose();
                this.Writer.Dispose();
                this.SessionNetwork.TcpClient.Close();
            }
        }

        private void IdleTimeout(object sender, ElapsedEventArgs e)
        {
            StopSession();
        }

        /// <summary>
        /// Sends a response to the client.
        /// </summary>
        /// <param name="response">A string representing the response to the client.</param>
        /// <returns>Task</returns>
        public async Task SendResponseAsync(string response)
        {
            Console.WriteLine("Server >>>: {0}", response);
            await this.Writer.WriteLineAsync(response);
        }

        /// <summary>
        /// Listen for the clients request.
        /// </summary>
        /// <returns>A string representing the clients request.</returns>
        public async Task<string> ListenRequestAsync()
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
        /// Initiates the StartTls command negotiation.
        /// </summary>
        /// <returns>Task</returns>
        public async Task StartTlsAsync()
        {
            this.SessionNetwork.SslStream = new SslStream(this.SessionNetwork.NetworkStream, false);
            var protocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            await this.SessionNetwork.SslStream.AuthenticateAsServerAsync(
                this.ServerConfig.X509Certificate2, false, protocols, false);

            //this.Reader.Close();
            //this.Writer.Close();

            this.Reader = new StreamReader(this.SessionNetwork.SslStream, Encoding.UTF8);
            this.Writer = new StreamWriter(this.SessionNetwork.SslStream, Encoding.UTF8)
            {
                AutoFlush = true,
                NewLine = "\r\n"
            };
        }

        public void Reset()
        {
            this.SmtpCommands.Clear();
        }

        public void StopSession()
        {
            this.TokenSource.Cancel();
        }
    }
}