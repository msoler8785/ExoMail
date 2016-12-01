using ExoMail.Smtp.Server.Interfaces;

namespace ExoMail.Smtp.Server.Authentication
{
    public class SaslFactory
    {
        public static ISaslMechanism GetSaslMechanism(string mechanism)
        {
            mechanism = mechanism.ToUpper();

            switch (mechanism)
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