using ExoMail.Smtp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Models
{
    public abstract class EmailValidator
    {
        public SmtpSessionBase SmtpSession { get; set; }

        public EmailValidator(SmtpSessionBase smtpSession)
        {
            this.SmtpSession = smtpSession;
        }

        public abstract Task<bool> IsValid();
    }
}
