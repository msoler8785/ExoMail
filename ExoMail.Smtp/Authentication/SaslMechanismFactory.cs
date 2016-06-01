using ExoMail.Smtp.Interfaces;

namespace ExoMail.Smtp.Authentication
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