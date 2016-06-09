using ExoMail.QueueProcessor.Services;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Protocol;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Example
{
    public class FileMessageStore : IMessageStore
    {
        public async Task Save(
            MemoryStream memoryStream,
            SmtpReceivedHeader receivedHeader,
            IMessageEnvelope messageEnvelope
            )
        {
            var directory = InboundQueueProcessor.InboundQueuePath;
            var path = Path.Combine(directory, messageEnvelope.MessageId + ".eml");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                var headers = await receivedHeader.GetReceivedHeaders();

                await headers.CopyToAsync(stream);
                await memoryStream.CopyToAsync(stream);
            }

            messageEnvelope.SaveEnvelope(path);
        }
    }
}