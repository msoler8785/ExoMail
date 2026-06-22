using ExoMail.Smtp.Common.Interfaces;
using System.Collections.Generic;

namespace ExoMail.Smtp.QueueProcessor
{
    public interface IRouteRepository : IRepository<IMessageRoute>
    {
        HashSet<IMessageRoute> Routes { get; }
        void PersistChanges();
    }
}