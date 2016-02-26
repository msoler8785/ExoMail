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
        public List<IAuthorizedNetwork> AuthorizedNetworks { get; set; }
        public List<MailRecipientCollection> MailRecipients { get; set; }

        public SmtpRelaySession() { }

        public SmtpRelaySession(TcpClient tcpClient) 
            : this(tcpClient, CancellationToken.None) { }

        public SmtpRelaySession(TcpClient tcpClient, CancellationToken cancellationToken)
            : base(tcpClient, cancellationToken)
        {
            this.AuthorizedNetworks = new List<IAuthorizedNetwork>();
            this.MailRecipients = new List<MailRecipientCollection>();
        }



        public override async Task BeginSessionAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> AuthorizeUser(SmtpCommand smtpCommand)
        {
            throw new NotImplementedException();
        }

        public override void StopSession()
        {
            throw new NotImplementedException();
        }
    }
}
