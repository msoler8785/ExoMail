using ExoMail.Smtp.Client.Network;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpEhloCommand : SmtpHeloCommand
    {
        public SmtpEhloCommand(SmtpClient smtpClient)
            : base(smtpClient)
        {
        }

        public override string CommandVerb => "EHLO";
    }
}