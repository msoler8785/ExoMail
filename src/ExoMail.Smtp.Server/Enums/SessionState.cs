using System;

namespace ExoMail.Smtp.Server.Enums
{
    [Flags]
    public enum SessionState
    {
        EhloNeeded = 1,
        StartTlsNeeded = 2,
        MailNeeded = 4,
        RcptNeeded = 8,
        DataNeeded = 16,
        AuthNeeded = 32
    }
}