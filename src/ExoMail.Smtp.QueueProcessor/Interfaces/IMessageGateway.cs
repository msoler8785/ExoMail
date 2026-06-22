using ExoMail.Smtp.Common.Interfaces;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public interface IMessageGateway
    {
        Task EnqueueMessage(IQueuedMessage queuedMessage, string domain);

        IGatewayOptions GatewayOptions { get; set; }
    }
}