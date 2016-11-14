using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public sealed class SmtpStartTlsCommand : SmtpCommandBase
    {
        public SmtpStartTlsCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }


        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count == 0;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
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

                    case SessionState.MailNeeded:
                    case SessionState.StartTlsNeeded:
                        this.IsValid = true;
                        response = SmtpResponse.StartTls;
                        break;

                    default:
                        response = SmtpResponse.BadCommand;
                        break;
                }
            }
           
            return response;
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        public override async Task ProcessCommandAction()
        {
            if (this.IsValid)
            {
                await this.SmtpSession.StartTlsAsync();
                this.SmtpSession.SessionState = SessionState.EhloNeeded;
                this.SmtpSession.SmtpCommands.Add(this);
            }
        }
    }
}