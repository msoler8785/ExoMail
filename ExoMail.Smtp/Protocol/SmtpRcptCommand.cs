using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpRcptCommand : SmtpCommandBase
    {

        public SmtpRcptCommand(string command, List<string> arguments)
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
                        response = SmtpResponse.SenderFirst;
                        break;
                    case SessionState.RcptNeeded:
                    case SessionState.DataNeeded:
                        response = GetRcptResponse();
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
        private string GetRcptResponse()
        {
            string response;
            var regex = Regex.Match(this.Arguments[0], @"TO:<(.*)>", RegexOptions.IgnoreCase);
            var validFormat = regex.Success;
            var recipient = regex.Groups[1].Value;


            if (validFormat)
            {
                if (this.SmtpSession.UserStore.IsValidRecipient(recipient))
                {
                    this.IsValid = true;
                    this.SmtpSession.SessionState = SessionState.DataNeeded;
                    this.SmtpSession.SmtpCommands.Add(this);
                    response = SmtpResponse.RecipientOK;
                }
                else
                {
                    response = SmtpResponse.MailboxUnavailable;
                }
            }
            else
            {
                response = SmtpResponse.InvalidSenderName;
            }
            return response;
        }

    }


}