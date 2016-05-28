using ARSoft.Tools.Net;
using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpHeloCommand : SmtpCommandBase
    {
        public SmtpHeloCommand(string command, List<string> arguments)
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


                    string ptrRecord = await this.SmtpSession.SessionNetwork.PtrRecordAsync;
                    response = String.Format(SmtpResponse.Hello, this.SmtpSession.ServerConfig.HostName, ptrRecord);
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
    }
}