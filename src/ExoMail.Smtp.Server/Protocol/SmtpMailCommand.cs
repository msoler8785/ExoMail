using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpMailCommand : SmtpCommandBase
    {
        public override SessionState AllowedStates => SessionState.MailNeeded | SessionState.RcptNeeded;

        private static readonly string[] _validArgs = { "BODY", "SIZE", "FROM" };
        private bool EightBitMime { get; set; }
        private bool HasSize { get; set; }
        private int Size { get; set; }
        private MailAddress Sender { get; set; }

        public override async Task ExecuteAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {
                this.SmtpSession.SmtpStateTable.SessionState = SessionState.RcptNeeded;
                this.SmtpSession.SmtpStateTable.Sender = Sender;
                this.SmtpSession.SmtpCommands.Add(this);
            }
            await SmtpSession.SendResponseAsync(response);
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            if (SmtpSession.SmtpStateTable.Sender != null)
            {
                argumentsResponse = SmtpResponse.SenderAlreadySpecified;
                return false;
            }

            if (!ArgsValid(out argumentsResponse))
            {
                return false;
            }

            if (!SizeValid(out argumentsResponse))
            {
                return false;
            }

            if (!SenderValid(out argumentsResponse))
            {
                return false;
            }

            if (Arguments.ContainsKey("BODY"))
            {
                string value = Arguments["BODY"].ToUpper();
                switch (value)
                {
                    case "8BITMIME":
                        EightBitMime = true;
                        argumentsResponse = String.Format(SmtpResponse.SenderAnd8BitOK, Sender.FullAddress);
                        break;

                    case "7BIT":
                        EightBitMime = false;
                        break;

                    default:
                        argumentsResponse = SmtpResponse.ArgumentUnrecognized;
                        return false;
                }
            }

            return true;
        }

        private bool ArgsValid(out string response)
        {
            if (Arguments.All(arg => _validArgs.Contains(arg.Key, StringComparer.OrdinalIgnoreCase)))
            {
                response = String.Empty;
                return true;
            }
            else
            {
                response = SmtpResponse.ArgumentUnrecognized;
                return false;
            }
        }

        private bool SenderValid(out string response)
        {
            string senderArg;
            MailAddress senderAddress;

            if (Arguments.TryGetValue("FROM", out senderArg) && MailAddress.TryParse(senderArg, true, out senderAddress))
            {
                response = String.Format(SmtpResponse.SenderOK, senderAddress.FullAddress);
                Sender = senderAddress;
                return true;
            }
            else
            {
                response = SmtpResponse.InvalidSenderName;
                return false;
            }
        }

        private bool SizeValid(out string response)
        {
            int size = 0;
            response = String.Empty;

            if (!Arguments.ContainsKey("SIZE"))
            {
                Size = 0;
                return true;
            }

            if (Arguments.TryGetValue("SIZE", out size))
            {
                Size = size;
                HasSize = true;

                if (Size > SmtpSession._serverConfig.MaxMessageSize)
                {
                    response = SmtpResponse.SystemSizeExceeded;
                    return false;
                }

                return true;
            }

            response = SmtpResponse.ArgumentUnrecognized;
            return false;
        }

        //public override bool ValidateArgs(out string argumentsResponse)
        //{
        //    argumentsResponse = String.Empty;

        //    if (SmtpSession.SmtpStateTable.Sender != null)
        //    {
        //        argumentsResponse = SmtpResponse.SenderAlreadySpecified;
        //        return false;
        //    }

        //    bool argsValid = Arguments.All(arg => _validArgs.Contains(arg.Key, StringComparer.OrdinalIgnoreCase));
        //    bool argsCount = Arguments.Count > 0 && Arguments.Count < 4;
        //    int size = 0;

        //    string sender;
        //    bool senderValid = Arguments.TryGetValue("FROM", out sender);

        //    if (argsValid && argsCount)
        //    {
        //        if (senderValid)
        //        {
        //            _sender = sender.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        //            argumentsResponse = String.Format(SmtpResponse.SenderOK, _sender);
        //        }
        //        else
        //        {
        //            argumentsResponse = SmtpResponse.InvalidSenderName;
        //            return false;
        //        }

        //        if (Arguments.TryGetValue("SIZE", out size))
        //        {
        //            _sizeBytes = size;
        //            _hasSize = true;

        //            if (_sizeBytes > SmtpSession.ServerConfig.MaxMessageSize)
        //            {
        //                argumentsResponse = SmtpResponse.SystemSizeExceeded;
        //                return false;
        //            }
        //        }

        //        if (Arguments.ContainsKey("BODY"))
        //        {
        //            var body = Arguments["BODY"];
        //            _8bitmime = body.Equals("8BITMIME");

        //            if (_8bitmime)
        //                argumentsResponse = String.Format(SmtpResponse.SenderAnd8BitOK, _sender);
        //        }

        //        return true;
        //    }
        //    else
        //    {
        //        argumentsResponse = SmtpResponse.ArgumentUnrecognized;
        //    }
        //    return false;
        //}
    }
}