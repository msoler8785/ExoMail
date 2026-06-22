using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpRsetCommand : SmtpCommandBase, ISmtpCommand
    {
        public SmtpRsetCommand(SmtpClient smtpClient)
            : base(smtpClient)
        {
        }

        public override string CommandVerb => "RSET";

        public async Task Invoke()
        {
            await base.SmtpClient.SendCommandAsync(CommandVerb);
        }
    }
}