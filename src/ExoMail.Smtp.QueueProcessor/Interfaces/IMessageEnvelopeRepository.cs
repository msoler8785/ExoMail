using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public interface IMessageEnvelopeRepository : IRepository<IMessageEnvelope>
    {
        IEnumerable<IMessageEnvelope> FindAll(string messageId);

        Task ArchiveAsync(IMessageEnvelope envelope);
    }
}