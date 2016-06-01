namespace ExoMail.Smtp.Interfaces
{
    public interface ISaslMechanism
    {
        string UserName { get; set; }
        string Password { get; set; }
        bool IsCompleted { get; }
        bool CanInitiateChallenge { get; }
        bool IsAuthenticated { get; }
        string SaslMechanism { get; set; }
        int Step { get; set; }

        string GetChallenge();

        void ParseResponse(string response);
    }
}