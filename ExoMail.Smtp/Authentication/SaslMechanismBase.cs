namespace ExoMail.Smtp.Authentication
{
    /// <summary>
    /// Simple Authentication and Security Layer (SASL)
    /// <see cref="https://tools.ietf.org/html/rfc4422"/>
    /// </summary>
    public abstract class SaslMechanismBase
    {
        // The UserName of the UserIdentity to authenticate
        public string UserName { get; set; }

        // The Password of the UserIdentity to authenticate
        public string Password { get; set; }

        // The name of the SASL Mechanism
        public string SaslMechanism { get; set; }

        // True if the exchange is completed
        public bool IsCompleted { get; private set; }

        // True if the mechanism can send an initial response
        public bool CanInitiateChallenge { get; private set; }

        // The current step in the challenge response process
        public int Step { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return UserManager.GetUserManager.IsUserAuthenticated(this.UserName, this.Password);
            }
        }

        public SaslMechanismBase()
        {
            this.IsCompleted = false;
        }

        protected void SetCompleted(bool isCompleted)
        {
            this.IsCompleted = isCompleted;
        }

        protected void SetCanInitiateChallenge(bool isInitiator)
        {
            this.CanInitiateChallenge = isInitiator;
        }
    }
}