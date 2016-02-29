using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Exceptions
{
    public class SaslException : Exception
    {
        public SaslException()
            :base()
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
