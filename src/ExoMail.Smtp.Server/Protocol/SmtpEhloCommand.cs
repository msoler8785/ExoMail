using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Server.Enums;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpEhloCommand : SmtpHeloCommand
    {
        public SmtpEhloCommand()
            : base()
        {
        }

        public override async Task ExecuteAsync()
        {
            if (ValidateArgs(out string response))
            {
                SmtpSession.Reset();
                SmtpSession.SmtpStateTable.EhloCompleted = true;
                SmtpSession.SessionNetwork.RemoteDomainName = this.SendingHost;
                //SmtpSession.MessageEnvelope.SetSenderDomain(this.SendingHost.ToString().TrimEnd('.'));

                if (SmtpSession._serverConfig.IsEncryptionRequired && !SmtpSession.SessionNetwork.IsEncrypted)
                    SmtpSession.SmtpStateTable.SessionState = SessionState.StartTlsNeeded;
                else if (SmtpSession._serverConfig.IsAuthRequired && !SmtpSession.SmtpStateTable.IsAuthenticated)
                    SmtpSession.SmtpStateTable.SessionState = SessionState.AuthNeeded;
                else
                    SmtpSession.SmtpStateTable.SessionState = SessionState.MailNeeded;
            }
            response = GetEhloResponseMessage();

            await SmtpSession.SendResponseAsync(response);
        }

        private string GetEhloResponseMessage()
        {
            var sb = new StringBuilder();
            //string ptrRecord = await this.SmtpSession.SessionNetwork.PtrRecordAsync;

            sb.AppendLineFormat("250-{0} Hello [{1}]", this.SmtpSession._serverConfig.HostName, this.SendingHost);
            sb.AppendLineFormat("250-SIZE {0}", this.SmtpSession._serverConfig.MaxMessageSize);

            if (!this.SmtpSession.IsAuthenticated)
            {
                sb.AppendLineFormat("250-AUTH LOGIN PLAIN");
            }

            if (!this.SmtpSession.SessionNetwork.IsEncrypted && this.SmtpSession._serverConfig.X509Certificate2 != null)
            {
                sb.AppendLine("250-STARTTLS");
            }

            sb.AppendLineFormat("250-ENHANCEDSTATUSCODES");
            sb.AppendLineFormat("250-PIPELINING");
            sb.AppendLineFormat("250-8BITMIME");
            sb.Append("250 HELP");

            return sb.ToString();
        }
    }
}