using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queue");
            var path = Path.Combine(directory, Guid.NewGuid().ToString() + ".eml");

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
