using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Enums
{
    public enum SessionState
    {
        EhloNeeded,
        StartTlsNeeded,
        MailNeeded,
        RcptNeeded,
        DataNeeded,
        AuthNeeded
    }
}
