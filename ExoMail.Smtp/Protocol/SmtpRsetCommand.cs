using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override bool ArgumentsValid
        {
            get
            {
                return this.Arguments.Count == 0;
            }
        }

        private string GetResponse()
        {
            string response;

            if (this.ArgumentsValid)
            {
                response = SmtpResponse.Resetting;
            }
            else
            {
                response = SmtpResponse.ArgumentUnrecognized;
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