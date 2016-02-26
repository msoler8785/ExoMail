using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Enums
{
    public enum SmtpCommandType
    {
        EHLO,
        HELO,
        MAIL,
        RCPT,
        DATA,
        RSET,
        QUIT,
        NOOP,
        STARTTLS,
        AUTH,
        HELP,
        SAML,
        VRFY,
        TURN,
        INVALID
    }
}
