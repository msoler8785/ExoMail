using System;

namespace ExoMail.Smtp.Server.Exceptions
{
    public class SmtpException : Exception
    {
        public SmtpException()
            : base()
        {
        }

        public SmtpException(string message)
            : base(message)
        {
        }

        public SmtpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}