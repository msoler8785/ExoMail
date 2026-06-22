using System;

namespace ExoMail.Smtp.Client.Enums
{
    [Flags]
    public enum ServerExtensions
    {
        EHLO = 1,
        STARTTLS = 2,
        SIZE = 4,
        EIGHTBITMIME = 8,
        ENHANCEDSTATUSCODES = 16,
        AUTH = 32,
        PIPELINING = 64,
        SMTPUTF8 = 128,
        DSN = 256,
    }
}