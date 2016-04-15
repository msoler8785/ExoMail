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
