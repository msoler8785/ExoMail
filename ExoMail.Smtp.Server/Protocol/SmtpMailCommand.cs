using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpMailCommand : SmtpCommandBase
    {
        public string MailFrom { get; set; }

        public bool EightBit { get; set; }

        public SmtpMailCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
            EightBit = false;
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count > 0 && this.Arguments.Count <= 2;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;
            return result;
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        private string GetResponse()
        {
            string response;

            if (ValidateArgs(out response))
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

            return response;
        }

        private string GetMailResponse()
        {
            string response;
            bool bodyValid = true;

            var from = Regex.Match(this.Arguments[0], @"FROM:<(.*)>$", RegexOptions.IgnoreCase);

            if (this.Arguments.Count == 2)
            {
                var body = Regex.Match(this.Arguments[1], @"(BODY)=(7BIT|8BITMIME)$", RegexOptions.IgnoreCase);
                this.EightBit = body.Groups[2].Value.ToUpper() == "8BITMIME";
                bodyValid = body.Success;
            }

            if (from.Success && bodyValid)
            {
                this.MailFrom = from.Groups[1].Value;
                this.SmtpSession.MessageEnvelope.SetSenderAddress(this.MailFrom);

                // TODO: Implement sender validation logic here.
                this.IsValid = true;
                this.SmtpSession.SessionState = SessionState.DataNeeded;
                this.SmtpSession.SmtpCommands.Add(this);
                response = this.EightBit ? SmtpResponse.SenderAnd8BitOK : SmtpResponse.SenderOK;
            }
            else
            {
                response = SmtpResponse.InvalidSenderName;
            }
            return response;
        }
    }
}