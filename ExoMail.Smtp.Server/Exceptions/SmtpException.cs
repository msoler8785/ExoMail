using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Exceptions
{
    [Serializable]
    public class SmtpException : Exception
    {

        public SmtpException()
            :base()
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
