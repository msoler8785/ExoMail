using ARSoft.Tools.Net;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpHeloCommand : SmtpCommandBase
    {
        public DomainName SendingHost { get; set; }

        public SmtpHeloCommand(string command, List<string> arguments)
            : base()
        {
            Command = command;
            Arguments = arguments;
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

            DomainName domainName;

            bool validDomain = DomainName.TryParse(this.Arguments[0], out domainName);
            if (!validDomain)
                argumentsResponse = SmtpResponse.InvalidDomainName;

            this.SendingHost = domainName;

            bool result = validDomain && count;
            return result;
        }

        public override async Task<string> GetResponseAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {

                this.SmtpSession.Reset();
                this.IsValid = true;
                this.SmtpSession.SessionNetwork.RemoteDomainName = this.SendingHost;
                this.SmtpSession.MessageEnvelope
                    .SetSenderDomain(this.SendingHost.ToString().TrimEnd('.'));

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


                string ptrRecord = await this.SmtpSession.SessionNetwork.PtrRecordAsync;
                response = String.Format(SmtpResponse.Hello, this.SmtpSession.ServerConfig.HostName, ptrRecord);

            }

            return response;
        }
    }
}