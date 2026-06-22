using System.Threading.Tasks;

namespace ExoMail.Smtp.Service
{
    public interface IStartup
    {
        Task Initialize();
    }
}