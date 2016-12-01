using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Exceptions
{
    [Serializable]
    public class SmtpCommandException : Exception
    {

        public SmtpCommandException()
            :base()
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
