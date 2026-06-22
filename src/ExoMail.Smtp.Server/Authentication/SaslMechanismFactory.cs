using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;

namespace ExoMail.Smtp.Server.Authentication
{
    public class SaslFactory
    {
        private readonly IVirtualServerRepository _userManager;

        public SaslFactory(IVirtualServerRepository userManager)
        {
            _userManager = userManager;
        }

        public ISaslMechanism GetSaslMechanism(string mechanism)
        {
            mechanism = mechanism.ToUpper();

            switch (mechanism)
            {
                case "PLAIN":
                    return new PlainSaslMechanism(_userManager);

                case "LOGIN":
                    return new LoginSaslMechanism(_userManager);

                default:
                    return null;
            }
        }
    }
}