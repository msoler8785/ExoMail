using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpMailCommand : SmtpCommandBase
    {
        public SmtpMailCommand(string command, List<string> arguments)
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
            return await Task.Run(() => GetResponse());
        }

        private string GetResponse()
        {
            string response;

            if (this.ArgumentsValid)
            {
                switch (this.SmtpSession.SessionState)
                {
                    case SessionState.EhloNeeded:
                        response = SmtpResponse.SendHelloFirst;
                        break;
                    case SessionState.StartTlsNeeded:
                        response = SmtpResponse.StartTlsFirst;
                        break;
                    case SessionState.MailNeeded:
                        if (this.Arguments[0].ToUpper().Contains("FROM:"))
                        {
                            this.SmtpSession.SmtpCommands.Add(this);
                            this.SmtpSession.SessionState = SessionState.RcptNeeded;
                            response = SmtpResponse.SenderOK;
                        }
                        else
                        {
                            response = SmtpResponse.InvalidSenderName;
                        }
                        break;
                    case SessionState.RcptNeeded:
                        response = SmtpResponse.SenderAlreadySpecified;
                        break;
                    case SessionState.AuthNeeded:
                        response = SmtpResponse.AuthRequired;
                        break;
                    default:
                        response = SmtpResponse.BadCommand;
                        break;
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