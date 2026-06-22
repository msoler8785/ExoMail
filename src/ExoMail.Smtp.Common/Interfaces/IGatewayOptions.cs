namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IGatewayOptions
    {
        int MaxClientsPerDomain { get; set; }
        int MaxMessagesPerSession { get; set; }
        int MessageDeferralTime { get; set; }
        int MaxMessageRetries { get; set; }
    }
}