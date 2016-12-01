namespace ExoMail.Smtp.Server.Interfaces
{
    public interface ISaslMechanism
    {
        string UserName { get; set; }
        string Password { get; set; }
        bool IsCompleted { get; }
        bool CanSendInitialResponse { get; }
        bool IsInitiator { get; }
        bool IsAuthenticated { get; }
        string SaslMechanism { get; set; }
        int Step { get; set; }

        string GetChallenge();

        void ParseResponse(string response);
    }
}