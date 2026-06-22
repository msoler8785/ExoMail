using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service
{
    public interface ISmtpServerBuilder
    {
        IEnumerable<Task> Build(CancellationToken token);
    }
}