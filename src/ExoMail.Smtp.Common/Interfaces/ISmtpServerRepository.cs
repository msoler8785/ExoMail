using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface ISmtpServerRepository : IRepository<ISmtpServerConfig>
    {
        Task Start();
        Task Stop();
    }
}
