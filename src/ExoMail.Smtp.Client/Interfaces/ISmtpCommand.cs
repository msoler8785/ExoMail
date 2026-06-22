using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Interfaces
{
    internal interface ISmtpCommand
    {
        IExecutionResult Result { get; }

        Task RetrieveResult();

        Task Invoke();
    }
}