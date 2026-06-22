using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SessionInitiation
    {
        private SmtpClient _smtpClient;

        public SessionInitiation(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task<IExecutionResult> InvokeCommands()
        {
            ISmtpCommand ehlo = new SmtpEhloCommand(_smtpClient);
            await ehlo.Invoke();
            await ehlo.RetrieveResult();

            if (ehlo.Result.ResponseCode == 554)
            {
                // Failback to HELO command.
                ehlo = new SmtpHeloCommand(_smtpClient);
                await ehlo.RetrieveResult();
                _smtpClient.ServerExtensions &= ~ServerExtensions.EHLO;
            }

            return ehlo.Result;
        }
    }
}