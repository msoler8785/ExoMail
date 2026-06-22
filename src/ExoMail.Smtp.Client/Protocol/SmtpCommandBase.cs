using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal abstract class SmtpCommandBase
    {
        public IExecutionResult Result { get; set; }
        public SmtpClient SmtpClient { get; private set; }

        public SmtpCommandBase(SmtpClient smtpClient)
        {
            SmtpClient = smtpClient;
        }

        public abstract string CommandVerb { get; }

        public virtual async Task RetrieveResult()
        {
            string response = await SmtpClient.ListenResponseAsync();
            Result = ExecutionResult.Parse(response);
        }
    }
}