using System.Threading.Tasks;
using ExoMail.Smtp.Client.Network;

namespace ExoMail.Smtp.QueueProcessor
{
    public interface ISmtpClientResolver
    {
        Task<SmtpClient> TryResolveAsync(string domain);
    }
}