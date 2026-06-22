using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpQuitCommand : SmtpCommandBase, ISmtpCommand
    {
        public SmtpQuitCommand(SmtpClient smtpClient)
            : base(smtpClient)
        {
        }

        public override string CommandVerb => "QUIT";

        public async Task Invoke()
        {
            await base.SmtpClient.SendCommandAsync(CommandVerb);
        }
    }
}