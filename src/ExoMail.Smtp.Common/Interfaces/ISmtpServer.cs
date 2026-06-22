using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface ISmtpServer
    {
        bool IsRunning { get; }
        Task StartAsync(ISmtpServerConfig serverConfig, CancellationToken token);
    }
}