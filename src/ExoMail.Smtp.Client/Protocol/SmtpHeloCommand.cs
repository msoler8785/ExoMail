using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpHeloCommand : SmtpCommandBase, ISmtpCommand
    {
        public SmtpHeloCommand(SmtpClient smtpClient)
            : base(smtpClient)
        {
        }

        public override string CommandVerb => "HELO";

        public virtual async Task Invoke()
        {
            string commandLine = $"{CommandVerb} {base.SmtpClient.HostName}";
            await base.SmtpClient.SendCommandAsync(commandLine);
        }
    }
}