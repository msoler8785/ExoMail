using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Interfaces
{
    internal interface ISmtpSessionPhase
    {
        Task<IExecutionResult> InvokeCommands();
    }
}