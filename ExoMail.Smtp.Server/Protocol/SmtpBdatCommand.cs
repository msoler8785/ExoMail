using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Security;
using System.IO;
using Microsoft.IO;
using System.Text;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpBdatCommand : SmtpCommandBase
    {
        /// <summary>
        /// The size of this BDAT Chunk in bytes.
        /// </summary>
        public uint ChunkSize { get; set; }

        /// <summary>
        /// Is this the "LAST" chunk.
        /// </summary>
        public bool IsLast { get; set; }

        public SmtpBdatCommand(string command, List<string> arguments)
            : base()
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ValidateArgs(out string response)
        {
            response = String.Empty;

            if (this.Arguments.Count > 0 && this.Arguments.Count <= 2)
            {
                uint chunkSize;
                bool validChunkSize = UInt32.TryParse(this.Arguments[0], out chunkSize);

                if (validChunkSize)
                {
                    this.ChunkSize = chunkSize;
                }

                if (this.Arguments.Count == 2)
                {
                    this.IsLast = this.Arguments[1].ToUpper() == "LAST";
                    if (this.IsLast)
                        return this.IsLast && validChunkSize;
                }

                return validChunkSize;
            }

            response = SmtpResponse.ArgumentUnrecognized;
            return false;
        }

        private string GetResponse()
        {
            string response;
            if (ValidateArgs(out response))
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


        public override Task<string> GetResponseAsync()
        {
            return Task.Run(() => SmtpResponse.CommandNotImplemented);
            throw new NotImplementedException();
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

        private Task<string> ReceiveDataAsync(Stream stream)
        {
            throw new NotImplementedException();

            using (var memoryStream = new RecyclableMemoryStreamManager().GetStream())
            using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
            {

                reader.Read();
                
            }

        }
    }
}