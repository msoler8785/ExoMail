using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    internal class SmtpAuthCommand : SmtpCommandBase
    {
        public SmtpAuthCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ArgumentsValid
        {
            get
            {
                return this.Arguments.Count > 1;
            }
        }

        public override async Task<string> GetResponseAsync()
        {
            string response;

            if (this.ArgumentsValid)
            {
                if (this.SmtpSession.IsAuthenticated)
                {
                    response = SmtpResponse.AlreadyAuthenticated;
                }
                else
                {
                    switch (this.SmtpSession.SessionState)
                    {
                        case Enums.SessionState.EhloNeeded:
                            response = SmtpResponse.SendHelloFirst;
                            break;
                        case Enums.SessionState.StartTlsNeeded:
                            response = SmtpResponse.StartTlsFirst;
                            break;
                        case Enums.SessionState.AuthNeeded:
                            response = await GetAuthResponse();
                            break;
                        default:
                            response = SmtpResponse.BadCommand;
                            break;
                    }
                }
            }
            else
            {
                response = SmtpResponse.ArgumentUnrecognized;
            }
            return response;
        }

        private async Task<string> GetAuthResponse()
        {
            var saslMechanism = this.SmtpSession
                .SaslMechanisms
                .FirstOrDefault(x => x.SaslMechanism.ToUpper() == this.Arguments[0].ToUpper());

            if (saslMechanism == null)
            {
                return SmtpResponse.AuthNotSupported;
            }

            if (saslMechanism.IsInitiator && this.Arguments.Count == 1)
            {
                // Begin the Sasl challenge-response.
                while (!saslMechanism.IsCompleted)
                {
                    await this.SmtpSession.SendResponseAsync(saslMechanism.GetChallenge());
                    saslMechanism.ParseResponse(await this.SmtpSession.ListenRequestAsync());
                }
            }
            else if (!saslMechanism.IsInitiator && this.Arguments.Count == 2)
            {
                // TODO: Some SASL mechanisms are initiated by the client.  When we implement
                // those mechanism we will handle the logic here.
                throw new NotImplementedException("Sasl mechanism not implemented yet.");
            }
            else
            {
                return SmtpResponse.ArgumentUnrecognized;
            }
            this.SmtpSession.IsAuthenticated = saslMechanism.IsAuthenticated;

            if (saslMechanism.IsAuthenticated)
            {
                this.IsValid = true;
                this.SmtpSession.SmtpCommands.Add(this);
                this.SmtpSession.SessionState = SessionState.MailNeeded;
                return SmtpResponse.AuthOk;
            }
            else
            {
                return SmtpResponse.AuthCredInvalid;
            }
        }
    }
}