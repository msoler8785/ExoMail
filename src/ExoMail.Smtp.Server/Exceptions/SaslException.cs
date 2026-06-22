using System;

namespace ExoMail.Smtp.Server.Exceptions
{
    public class SaslException : Exception
    {
        public SaslException()
            : base()
        {
        }

        public SaslException(string message)
            : base(message)
        {
        }

        public SaslException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}