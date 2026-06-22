using ExoMail.Smtp.Common.Interfaces;

namespace ExoMail.Smtp.Service.Configuration
{
    public class GatewayOptions : IGatewayOptions
    {
        public int MaxClientsPerDomain { get; set; }
        public int MaxMessagesPerSession { get; set; }
        public int MessageDeferralTime { get; set; }
        public int MaxMessageRetries { get; set; }

        public GatewayOptions()
        {
        }

        public static GatewayOptions Default()
        {
            return new GatewayOptions()
            {
                MaxClientsPerDomain = 20,
                MaxMessagesPerSession = 100,
                MessageDeferralTime = 300,
                MaxMessageRetries = 20
            };
        }
    }
}