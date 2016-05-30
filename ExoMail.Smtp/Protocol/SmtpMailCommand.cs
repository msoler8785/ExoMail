using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                        response = GetMailResponse();
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

        private string GetMailResponse()
        {
            string response;
            var regex = Regex.Match(this.Arguments[0], @"FROM:<(.*)>", RegexOptions.IgnoreCase);
            var validFormat = regex.Success;
            var sender = regex.Groups[1].Value;
            this.SmtpSession.MessageEnvelope.SetSenderEmail(sender);

            if (validFormat)
            {
                // TODO: Implement sender validation logic here.
                this.IsValid = true;
                this.SmtpSession.SessionState = SessionState.DataNeeded;
                this.SmtpSession.SmtpCommands.Add(this);
                response = SmtpResponse.RecipientOK;
            }
            else
            {
                response = SmtpResponse.InvalidSenderName;
            }
            return response;
        }
    }
}