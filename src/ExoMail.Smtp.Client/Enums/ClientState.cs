namespace ExoMail.Smtp.Client
{
    public enum ClientState
    {
        Pending,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
    }
}