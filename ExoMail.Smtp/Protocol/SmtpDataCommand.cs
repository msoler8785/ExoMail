using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Extensions;
using ExoMail.Smtp.Utilities;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpDataCommand : SmtpCommandBase
    {
        public SmtpDataCommand(string command, List<string> arguments)
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
                switch (this.SmtpSession.SessionState)
                {
                    case SessionState.DataNeeded:
                        response = SmtpResponse.StartInput;
                        break;

                    case SessionState.EhloNeeded:
                        response = SmtpResponse.SendHelloFirst;
                        break;

                    case SessionState.StartTlsNeeded:
                        response = SmtpResponse.StartTlsFirst;
                        break;

                    case SessionState.MailNeeded:
                        response = SmtpResponse.SenderFirst;
                        break;

                    case SessionState.RcptNeeded:
                        response = SmtpResponse.SenderAndRecipientFirst;
                        break;

                    case SessionState.AuthNeeded:
                        response = SmtpResponse.AuthRequired;
                        break;

                    default:
                        response = SmtpResponse.BadCommand;
                        break;
                }
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
            string response = await StartInputAsync();
            await this.SmtpSession.SendResponseAsync(response);
        }

        /// <summary>
        /// Enter into the data phase of the SmtpSession
        /// </summary>
        /// <returns>SmtpReply.Queued response code.</returns>
        private async Task<string> StartInputAsync()
        {
            string response;

            if (this.SmtpSession.IsEncrypted)
                response = await ReceiveDataAsync(this.SmtpSession.SessionNetwork.SslStream);
            else
                response = await ReceiveDataAsync(this.SmtpSession.SessionNetwork.NetworkStream);

            return response;
        }

        /// <summary>
        /// Receives the data portion of the SMTP transaction.
        /// </summary>
        /// <param name="stream">A NetworkStream or SslStream to read the data from.</param>
        /// <returns>A string indicating the result of the transaction.</returns>
        private async Task<string> ReceiveDataAsync(Stream stream)
        {
            using (var memoryStream = new RecyclableMemoryStreamManager().GetStream())
            using (var reader = new StreamReader(memoryStream, Encoding.ASCII))
            {
                // 8KB buffer for NetworkStream.
                byte[] buffer = new byte[8 * 1024];

                // Number of bytes read from NetworkStream.
                int bytesRead;

                do
                {
                    // Read the NetworkStream.
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, this.SmtpSession.Token);

                    // Write stream to MemoryStream for replay.
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);

                    // Rewind 5 bytes so that we can check for line terminator.
                    if (memoryStream.Length >= 5)
                        memoryStream.Seek(-5, SeekOrigin.End);
                }
                while (!reader.ReadToEnd().Contains(SmtpSession.TERMINATOR));

                // Truncate data terminator from stream. 
                memoryStream.SetLength(memoryStream.Length - 5);

                if (memoryStream.Length > this.SmtpSession.ServerConfig.MaxMessageSize)
                {
                    this.SmtpSession.Reset();
                    double maxMessageSize = this.SmtpSession.ServerConfig.MaxMessageSize / SmtpSession.ONE_MB;

                    Console.WriteLine("System >>>: Received: {0} bytes.  MaxMessageSize: {1} bytes.  Over by {2} bytes.",
                        memoryStream.Length,
                        this.SmtpSession.ServerConfig.MaxMessageSize,
                        memoryStream.Length - this.SmtpSession.ServerConfig.MaxMessageSize);

                    return String.Format(SmtpResponse.MessageSizeExceeded, maxMessageSize.ToString());
                }

                memoryStream.Position = 0;

                Console.WriteLine("System >>>: Received {0} bytes from {1}:{2}",
                    memoryStream.Length,
                    this.SmtpSession.SessionNetwork.RemoteEndPoint.Address,
                    this.SmtpSession.SessionNetwork.RemoteEndPoint.Port);

                SmtpReceivedHeader receivedHeader = new SmtpReceivedHeader(this.SmtpSession);

                await this.SmtpSession.MessageStore.Save(memoryStream, receivedHeader, this.SmtpSession.MessageEnvelope);
            }
            ResetTransactionBuffer();
            return SmtpResponse.Queued;
        }

        private void ResetTransactionBuffer()
        {
            this.SmtpSession.SmtpCommands =
                this.SmtpSession.SmtpCommands
                .Where(x => x.CommandType == SmtpCommandType.EHLO ||
                x.CommandType == SmtpCommandType.HELO).ToList();

            this.SmtpSession.SessionState = SessionState.MailNeeded;
            this.SmtpSession.MessageEnvelope.Reset();
        }
    }
}