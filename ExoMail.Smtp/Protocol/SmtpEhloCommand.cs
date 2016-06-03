using ARSoft.Tools.Net;
using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Extensions;
using ExoMail.Smtp.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpEhloCommand : SmtpCommandBase
    {
        public SmtpEhloCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ArgumentsValid
        {
            get
            {
                return this.Arguments.Count == 1;
            }
        }

        public override async Task<string> GetResponseAsync()
        {
            string response;

            if (this.ArgumentsValid)
            {
                DomainName domainName;
                var isValidDomain = DomainName.TryParse(this.Arguments[0], out domainName);

                if (isValidDomain)
                {
                    this.SmtpSession.Reset();
                    this.IsValid = true;
                    this.SmtpSession.SessionNetwork.RemoteDomainName = domainName;
                    this.SmtpSession.MessageEnvelope
                           .SetSenderDomain(domainName.ToString().TrimEnd('.'));

                    if (this.SmtpSession.ServerConfig.IsEncryptionRequired && !this.SmtpSession.IsEncrypted)
                    {
                        this.SmtpSession.SessionState = SessionState.StartTlsNeeded;
                    }
                    else if (this.SmtpSession.ServerConfig.IsAuthRequired)
                    {
                        this.SmtpSession.SessionState = SessionState.AuthNeeded;
                    }
                    else
                    {
                        this.SmtpSession.SessionState = SessionState.MailNeeded;
                    }

                    response = await GetEhloResponseMessage();
                }
                else
                {
                    response = SmtpResponse.InvalidDomainName;
                }
            }
            else
            {
                response = SmtpResponse.ArgumentUnrecognized;
            }

            return response;
        }

        private async Task<string> GetEhloResponseMessage()
        {
            var sb = new StringBuilder();
            string ptrRecord = await this.SmtpSession.SessionNetwork.PtrRecordAsync;

            sb.AppendLineFormat("250-{0} Hello [{1}]", this.SmtpSession.ServerConfig.HostName, ptrRecord);
            sb.AppendLineFormat("250-SIZE {0}", this.SmtpSession.ServerConfig.MaxMessageSize);

            if (!this.SmtpSession.IsAuthenticated)
            {
                sb.AppendLineFormat("250-AUTH LOGIN PLAIN");
            }

            if (!this.SmtpSession.IsEncrypted && this.SmtpSession.ServerConfig.X509Certificate2 != null)
            {
                sb.AppendLine("250-STARTTLS");
            }

            sb.AppendLine("250-ENHANCEDSTATUSCODES");
            sb.Append("250 HELP");

            return sb.ToString();
        }
    }
}