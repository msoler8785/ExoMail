using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Interfaces
{
    internal interface ISaslMechanism
    {
        // SmtpClient SmtpClient { get; }
        string UserName { get; }

        string Password { get; }
        bool IsComplete { get; }

        Task InitiateAuth();
    }
}