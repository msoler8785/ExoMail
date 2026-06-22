//using ExoMail.Smtp.Server.Enums;
//using ExoMail.Smtp.Server.Utilities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace ExoMail.Smtp.Server.Protocol
//{
//    public sealed class SmtpMailCommandOld : SmtpCommandBase
//    {
//        public string MailFrom { get; set; }

//        public bool EightBit { get; set; }

//        public override SessionState AllowedStates => SessionState.MailNeeded;

//        public SmtpMailCommandOld()
//            : base()
//        {
//            EightBit = false;
//        }

//        public override bool ValidateArgs(out string argumentsResponse)
//        {
//            argumentsResponse = String.Empty;
//            bool result = this.Arguments.Count > 0 && this.Arguments.Count <= 3;

//            if (!result)
//                argumentsResponse = SmtpResponse.ArgumentUnrecognized;
//            return result;
//        }

//        public override async Task ExecuteAsync()
//        {
//            string response;

//            if (ValidateArgs(out response))
//            {
//                response = GetMailResponse();
//            }

//            await SmtpSession.SendResponseAsync(response);

//        }

//        private bool IsEightBit()
//        {
//            var argCount = Arguments.Count;
//            if (argCount >= 2)
//            {
//                return Arguments[argCount-1].ToUpper().Contains("8BITMIME");
//            }
//            return false;
//        }

//        private bool IsValidRecipient()
//        {
//            return true;
//        }

//        private string GetMailResponse()
//        {
//            string response;
//            bool bodyValid = true;

//            var from = Regex.Match(this.Arguments[1], @"<(.*)>$", RegexOptions.IgnoreCase);

//            if (this.Arguments.Count == 3)
//            {
//                var body = Regex.Match(this.Arguments[2], @"(BODY)=(7BIT|8BITMIME)$", RegexOptions.IgnoreCase);
//                this.EightBit = body.Groups[2].Value.ToUpper() == "8BITMIME";
//                bodyValid = body.Success;
//            }

//            if (from.Success && bodyValid)
//            {
//                this.MailFrom = from.Groups[1].Value;
//                this.SmtpSession.MessageEnvelope.SetSenderAddress(this.MailFrom);

//                // TODO: Implement sender validation logic here.
//                this.IsValid = true;
//                this.SmtpSession.SessionState = SessionState.RcptNeeded;
//                this.SmtpSession.SmtpCommands.Add(this);
//                response = this.EightBit ? SmtpResponse.SenderAnd8BitOK : SmtpResponse.SenderOK;
//            }
//            else
//            {
//                response = SmtpResponse.InvalidSenderName;
//            }
//            return response;
//        }
//    }
//}