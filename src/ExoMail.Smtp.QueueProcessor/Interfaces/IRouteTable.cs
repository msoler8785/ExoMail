using ExoMail.Smtp.Common.Interfaces;

namespace ExoMail.Smtp.QueueProcessor
{
    public interface IRouteTable
    {
        IMessageRoute FindRoute(string domain, IVirtualServer virtualServer);
    }
}