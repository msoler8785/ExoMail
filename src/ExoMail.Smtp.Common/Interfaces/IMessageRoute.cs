using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IMessageRoute
    {
        string MessageRouteId { get; set; }
        int Metric { get; }
        bool IsMatch(string domain);
        string VirtualServerId { get; set; }

        Task EnqueueMesssage(IQueuedMessage queuedMessage, string key);
    }
}