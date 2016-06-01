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
                return this.Arguments.Count > 0;
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
                        case SessionState.EhloNeeded:
                            response = SmtpResponse.SendHelloFirst;
                            break;

                        case SessionState.StartTlsNeeded:
                            response = SmtpResponse.StartTlsFirst;
                            break;

                        case SessionState.AuthNeeded:
                        case SessionState.MailNeeded:
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
            try
            {
                if (saslMechanism.IsInitiator && this.Arguments.Count == 1)
                {
                    // Begin the Sasl challenge-response.
                    while (!saslMechanism.IsCompleted)
                    {
                        await this.SmtpSession.SendResponseAsync(saslMechanism.GetChallenge());
                        saslMechanism.ParseResponse(await this.SmtpSession.ListenRequestAsync());
                    }
                }
                else if (this.Arguments.Count == 2)
                {
                    while (!saslMechanism.IsCompleted)
                    {
                        saslMechanism.ParseResponse(await this.SmtpSession.ListenRequestAsync());

                        if (saslMechanism.IsCompleted)
                        {
                            break;
                        }

                        await this.SmtpSession.SendResponseAsync(saslMechanism.GetChallenge());
                    }
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
            catch (SaslException)
            {
                return SmtpResponse.AuthCredInvalid;
            }
        }
    }
}