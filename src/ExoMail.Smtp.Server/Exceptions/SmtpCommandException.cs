using System;

namespace ExoMail.Smtp.Server.Exceptions
{
    public class SmtpCommandException : Exception
    {
        public SmtpCommandException()
            : base()
        {
        }

        public SmtpCommandException(string message)
            : base(message)
        {
        }

        public SmtpCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}