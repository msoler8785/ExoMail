using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.Common.Utilities;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Interfaces;
using System.Collections.Generic;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpStateTable
    {
        public MailAddress Sender { get; set; }
        public HashSet<MailAddress> Recipients { get; set; }
        public SessionState SessionState { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool EhloCompleted { get; set; }
        public IUserIdentity User { get; set; }

        public SmtpStateTable()
        {
            Recipients = new HashSet<MailAddress>();
            IsAuthenticated = false;
            EhloCompleted = false;
            SessionState = SessionState.EhloNeeded;
            User = new SystemIdentity();
        }

        public void Reset()
        {
            Sender = null;
            Recipients.Clear();

            if (!EhloCompleted)
                SessionState = SessionState.EhloNeeded;
            else
                SessionState = SessionState.MailNeeded;
        }
    }
}