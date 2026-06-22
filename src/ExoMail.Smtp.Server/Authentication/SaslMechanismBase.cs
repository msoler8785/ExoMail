using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;

namespace ExoMail.Smtp.Server.Authentication
{
    /// <summary>
    /// Simple Authentication and Security Layer (SASL)
    /// <see cref="https://tools.ietf.org/html/rfc4422"/>
    /// </summary>
    public abstract class SaslMechanismBase
    {
        private readonly IVirtualServerRepository _userManager;

        // The UserName of the UserIdentity to authenticate
        public string UserName { get; set; }

        // The Password of the UserIdentity to authenticate
        public string Password { get; set; }

        // The name of the SASL Mechanism
        public string SaslMechanism { get; set; }

        // True if the exchange is completed
        public bool IsCompleted { get; private set; }

        // True if the mechanism can send an initial response
        public bool CanSendInitialResponse { get; private set; }

        // True if client can initiate challenge
        public bool IsInitiator { get; private set; }

        // The current step in the challenge response process
        public int Step { get; set; }

        public IUserIdentity User { get; private set; }

        public bool IsAuthenticated
        {
            get
            {
                IUserIdentity user = null;
                var authenticated = _userManager.TryAuthenticate(this.UserName, this.Password, out user);
                User = user;

                return authenticated;
            }
        }

        public SaslMechanismBase(IVirtualServerRepository userManager)
        {
            _userManager = userManager;
            this.IsCompleted = false;
            this.CanSendInitialResponse = false;
            this.IsInitiator = false;
        }

        protected void SetCompleted(bool isCompleted)
        {
            this.IsCompleted = isCompleted;
        }

        protected void SetCanSendInitialResponse(bool isInitiator)
        {
            this.CanSendInitialResponse = isInitiator;
        }
    }
}