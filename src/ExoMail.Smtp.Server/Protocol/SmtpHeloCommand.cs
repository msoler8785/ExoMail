using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpHeloCommand : SmtpCommandBase
    {
        public string SendingHost { get; set; }

        public SmtpHeloCommand()
            : base()
        {
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool count = this.Arguments.Count == 1;

            if (!count)
            {
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;
                return false;
            }
            string domain = this.Arguments.ElementAtOrDefault(0).Key;
            bool validDomain = !String.IsNullOrWhiteSpace(domain);

            if (!validDomain)
                argumentsResponse = SmtpResponse.InvalidDomainName;
            else
                this.SendingHost = domain;

            bool result = validDomain && count;
            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {
                if (ValidateArgs(out response))
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

                response = String.Format(SmtpResponse.Hello, this.SmtpSession._serverConfig.HostName, this.SendingHost);
            }

            await SmtpSession.SendResponseAsync(response);
        }
    }
}