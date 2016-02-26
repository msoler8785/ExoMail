using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Enums
{
    /// <summary>
    /// The type of SmtpServer
    /// <see cref="https://tools.ietf.org/html/rfc5321#section-2.3.10"/>
    /// </summary>
    public enum ServerType
    {
        Originator,
        Delivery,
        Relay,
        Gateway
    }
}
