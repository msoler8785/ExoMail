using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpStartTlsCommand : SmtpCommandBase, ISmtpCommand
    {
        public SmtpStartTlsCommand(SmtpClient smtpClient)
            : base(smtpClient)
        {
        }

        public override string CommandVerb => "STARTTLS";

        public async Task Invoke()
        {
            await SmtpClient.SendCommandAsync(CommandVerb);
        }
    }
}