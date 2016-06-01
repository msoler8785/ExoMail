using ExoMail.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Authentication
{
    public class SaslFactory
    {
        public static ISaslMechanism GetSaslMechanism(string mechanism)
        {
            mechanism = mechanism.ToUpper();

            switch(mechanism)
            {
                case "PLAIN":
                    return new PlainSaslMechanism();
                case "LOGIN":
                    return new LoginSaslMechanism();
                default:
                    return null;
            }
        }
    }
}
