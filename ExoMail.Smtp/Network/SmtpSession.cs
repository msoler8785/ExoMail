using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Models;
using ExoMail.Smtp.Utilities;
using Microsoft.IO;
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

namespace ExoMail.Smtp.Network
{
    /// <summary>
    /// An SmtpSession that handles all communications with an SMTP Client.
    /// </summary>
    public class SmtpSession : SmtpSessionBase
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SmtpSession() { }

        /// <summary>
        /// Constructor that initializes the minimum amount of properties for a session.
        /// </summary>
        /// <param name="tcpClient">A tcpclient to initiate the session with.</param>
        public SmtpSession(TcpClient tcpClient)
            : base(tcpClient, CancellationToken.None) { }

        /// <summary>
        /// Constructor that initializes the minimum amount of properties for a session
        /// and a cancellation token.
        /// </summary>
        /// <param name="tcpClient">A tcpclient to initiate the session with.</param>
        /// <param name="cancellationToken">A CancellationToken instance to cancel the session.</param>
        public SmtpSession(TcpClient tcpClient, CancellationToken cancellationToken)
            : base(tcpClient, cancellationToken) { }

        /// <summary>
        /// Begin the SmtpSession.
        /// </summary>
        /// <returns>Task</returns>
        public override async Task BeginSessionAsync()
        {
            await InitializeStreams();
            this.SmtpServer.SmtpSessions.Add(this);
            SmtpCommand smtpCommand;
            string response;
            this.Timer = new System.Timers.Timer(this.SmtpServer.ServerConfig.SessionTimeout);
            this.Timer.Elapsed += IdleTimeout;
            this.Timer.AutoReset = false;
            this.Timer.Enabled = true;

            try
            {
                if (IsValidatedSession())
                {
                    await SendResponseAsync(String.Format(SmtpResponse.Announcment, this.SmtpServer.ServerConfig.HostName));

                    while (!this.SmtpCommands.Any(x => x.Command == "QUIT"))
                    {
                        this.Token.ThrowIfCancellationRequested();

                        var commandLine = await ListenRequestAsync();

                        this.Timer.Stop();
                        this.Timer.Start();

                        smtpCommand = SmtpCommand.Parse(commandLine);

                        if (this.SmtpServer.ServerConfig.IsEncryptionRequired && !this.IsEncrypted)
                        {
                            response = WaitingForTls(smtpCommand);
                        }
                        else
                        {
                            response = await GetResponseAsync(smtpCommand);
                        }

                        await SendResponseAsync(response);

                        if (smtpCommand.CommandType == SmtpCommandType.STARTTLS)
                            await StartTlsAsync();
                    }
                }
                this.CancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Server >>>: Object disposed in BeginSessionAsync(): {0}", ex.Message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.Message);
                SendResponseAsync(SmtpResponse.TransactionFailed + ex.Message).RunSynchronously();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SendResponseAsync(SmtpResponse.LocalError).RunSynchronously();
            }
        }


        /// <summary>
        /// Stops this session and disposes unmanaged resources.
        /// </summary>
        public override void StopSession()
        {
            try
            {
                this.MessageStore = null;
                this.Timer.Dispose();
                this.Writer.Dispose();
                this.Reader.Dispose();
                if (this.TcpClient.Connected)
                {
                    this.TcpClient.Close();
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Error on stop, " + ex.Message);
            }
            finally
            {
                this.SmtpServer.SmtpSessions.Remove(this);
                //GC.Collect();
            }
        }
    }
}