using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpAuthCommand : SmtpCommandBase, ISmtpCommand
    {
        private ISaslMechanism SaslMechanism { get; set; }

        public SmtpAuthCommand(SmtpClient smtpClient, ISaslMechanism saslMechanism)
            : base(smtpClient)
        {
            SaslMechanism = saslMechanism;
        }

        public override string CommandVerb => "AUTH";

        public async Task Invoke()
        {
            await SaslMechanism.InitiateAuth();
        }
    }
}