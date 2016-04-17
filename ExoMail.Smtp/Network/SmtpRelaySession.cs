using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Models;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Network
{
    /// <summary>
    /// A "relay" SMTP system(usually referred to just as a "relay") receives
    /// mail from an SMTP client and transmits it, without modification to
    /// the message data other than adding trace information, to another SMTP
    /// server for further relaying or for delivery.
    /// <see cref="https://tools.ietf.org/html/rfc2821#section-2.3.8"/>
    /// </summary>

    public class SmtpRelaySession : SmtpSessionBase
    {
        public SmtpRelaySession() { }

        public SmtpRelaySession(TcpClient tcpClient) 
            : this(tcpClient, CancellationToken.None) { }

        public SmtpRelaySession(TcpClient tcpClient, CancellationToken cancellationToken)
            : base(tcpClient, cancellationToken)
        {
        }
    }
}
