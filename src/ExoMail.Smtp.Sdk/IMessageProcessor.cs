using ExoMail.Smtp.Common.Interfaces;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Sdk
{
    public interface IMessageProcessor
    {
        bool StopProcessing { get; }
        Task ProcessAsync(IQueuedMessage queuedMessage);
        bool Enabled { get; }
        int Priority { get; }
    }
}