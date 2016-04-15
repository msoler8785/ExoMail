﻿using ExoMail.Smtp.Enums;
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
    public class SmtpDeliverySession : SmtpSessionBase
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SmtpDeliverySession() { }

        /// <summary>
        /// Constructor that initializes the minimum amount of properties for a session.
        /// </summary>
        /// <param name="tcpClient">A tcpclient to initiate the session with.</param>
        public SmtpDeliverySession(TcpClient tcpClient)
            : base(tcpClient, CancellationToken.None)
        { }

        /// <summary>
        /// Constructor that initializes the minimum amount of properties for a session
        /// and a cancellation token.
        /// </summary>
        /// <param name="tcpClient">A tcpclient to initiate the session with.</param>
        /// <param name="cancellationToken">A CancellationToken instance to cancel the session.</param>
        public SmtpDeliverySession(TcpClient tcpClient, CancellationToken cancellationToken)
            : base(tcpClient, cancellationToken)
        {
            this.IsAuthenticatationRequired = false;
        }

        public override string GetRcptResponse(SmtpCommand smtpCommand)
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
            else if (!this.UserStore.IsValidRecipient(smtpCommand.Arguments.ElementAtOrDefault(0)))
            {
                response = SmtpResponse.MailboxUnavailable;
            }
            else
            {
                this.SmtpCommands.Add(smtpCommand);
                response = SmtpResponse.OK;
            }
            return response;
        }
    }
}