using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using ExoMail.Smtp.Common.Models;
using System.Threading.Tasks;
using ExoMail.Smtp.Common.Enums;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpRcptCommand : SmtpCommandBase, ISmtpCommand
    {
        public SmtpRcptCommand(SmtpClient smtpClient, IMessageEnvelope envelope)
            : base(smtpClient)
        {
            _envelope = envelope;
        }

        private IMessageEnvelope _envelope;

        public override string CommandVerb => "RCPT";

        public async Task Invoke()
        {
            string commandLine = $"RCPT TO:<{_envelope.Recipient.FullAddress}>";

            await SmtpClient.SendCommandAsync(commandLine);
        }

        public override async Task RetrieveResult()
        {
            await base.RetrieveResult();

            switch (Result.ResponseResult)
            {
                case ResponseResult.OK:
                    _envelope.Disposition = Disposition.Delivered;
                    break;

                case ResponseResult.TempFail:
                    _envelope.Disposition = Disposition.TempFail;
                    _envelope.LastResponse = Result.Response;
                    break;

                case ResponseResult.PermFail:
                case ResponseResult.Invalid:
                default:
                    _envelope.Disposition = Disposition.PermFail;
                    _envelope.LastResponse = Result.Response;
                    break;
            }
        }
    }
}