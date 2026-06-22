using System.Threading.Tasks;
using ExoMail.Smtp.Common.Interfaces;

namespace ExoMail.Smtp.QueueProcessor
{
    public interface IDefaultRouter
    {
        Task RouteAsync(IQueuedMessage queuedMessage);
    }
}