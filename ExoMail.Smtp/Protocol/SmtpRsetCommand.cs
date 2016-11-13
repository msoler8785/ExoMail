using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpRsetCommand : SmtpCommandBase
    {
        public SmtpRsetCommand(string command, List<string> arguments)
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
                response = SmtpResponse.Resetting;
            }

            return response;
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        public override async Task ProcessCommandAction()
        {
            await Task.Run(() =>
            {
                if (this.IsValid)
                {
                    this.SmtpSession.Reset();
                    this.SmtpSession.SessionState = SessionState.EhloNeeded;
                }
            });
        }
    }
}