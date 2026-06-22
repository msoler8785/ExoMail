using ExoMail.Smtp.Common.IO;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpDataCommand : SmtpCommandBase
    {
        private readonly ILogger<SmtpDataCommand> _logger;
        private readonly ILoggerFactory _loggerFactor;

        public override SessionState AllowedStates => SessionState.DataNeeded;

        public SmtpDataCommand(ILoggerFactory loggerFactory)
            : base()
        {
            _logger = loggerFactory.CreateLogger<SmtpDataCommand>();
            _loggerFactor = loggerFactory;
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count == 0;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;
            if (ValidateArgs(out response))
            {
                await SmtpSession.SendResponseAsync(SmtpResponse.StartInput);
                response = await StartInputAsync();
            }

            await SmtpSession.SendResponseAsync(response);
        }

        /// <summary>
        /// Enter into the data phase of the SmtpSession
        /// </summary>
        /// <returns>SmtpReply.Queued response code.</returns>
        private async Task<string> StartInputAsync()
        {
            string response;

            response = await ReceiveDataAsync(this.SmtpSession.SessionNetwork.Stream);

            return response;
        }

        /// <summary>
        /// Receives the data portion of the SMTP transaction.
        /// </summary>
        /// <param name="stream">A NetworkStream or SslStream to read the data from.</param>
        /// <returns>A string indicating the result of the transaction.</returns>
        private async Task<string> ReceiveDataAsync(Stream stream)
        {
            string messageId;

            using (var dataStream = new SmtpDataStream(stream, _loggerFactor))
            {
                var sw = new Stopwatch();
                sw.Start();

                try
                {
                    do await dataStream.ReadClient(SmtpSession.Token);
                    while (!dataStream.EndOfStream);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("A timeout occured while reading the SmtpDataStream from the client.");
                    return SmtpResponse.Timeout;
                }

                sw.Stop();

                // Truncate the data termination sequence.
                dataStream.MessageBuffer.SetLength(dataStream.MessageBuffer.Length - 5);

                messageId = await this.SmtpSession._messageStore.Save(this.SmtpSession, dataStream.MessageBuffer);
                _logger.LogInformation($"System: Received {dataStream.TotalRead} bytes in {sw.Elapsed.TotalSeconds} seconds.");
            }

            SmtpSession.Reset();
            return String.Format(SmtpResponse.Queued, messageId);
        }
    }
}