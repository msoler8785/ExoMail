using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using ExoMail.Smtp.Common.IO;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpDataCommand : SmtpCommandBase, ISmtpCommand
    {
        private readonly ILogger<SmtpDataCommand> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly MimeMessage _mimeMessage;

        private Stream _smtpStream;
        public override string CommandVerb => "DATA";

        public SmtpDataCommand(SmtpClient smtpClient, MimeMessage message, Stream smtpStream, ILoggerFactory loggerFactory)
                    : base(smtpClient)
        {
            _mimeMessage = message;
            _smtpStream = smtpStream;
            _logger = loggerFactory.CreateLogger<SmtpDataCommand>();
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke()
        {
            await SmtpClient.SendCommandAsync(CommandVerb);
        }

        public override async Task RetrieveResult()
        {
            string response = await SmtpClient.ListenResponseAsync();

            var input = ExecutionResult.Parse(response);

            if (input.ResponseResult == ResponseResult.Input)
            {
                SmtpClient.Timer.Change(300000, -1);
                var dataStream = new SmtpDataStream(_smtpStream, _loggerFactory);

                _mimeMessage.WriteTo(dataStream);
                await dataStream.TerminateDataStream();

                SmtpClient.Timer.Change(120000, -1);

                response = await SmtpClient.ListenResponseAsync();
            }

            Result = ExecutionResult.Parse(response);
        }
    }
}