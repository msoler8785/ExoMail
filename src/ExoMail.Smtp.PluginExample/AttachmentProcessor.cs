using System;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Sdk;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.PluginExample
{
    public class AttachmentProcessor : IMessageProcessor
    {
        private const string REPORT_PATH = "Reports";
        public bool StopProcessing => false;
        public bool Enabled => false;
        public int Priority => 100;

        public async Task ProcessAsync(IQueuedMessage queuedMessage)
        {
            await Task.Run(() =>
            {
                var attachments = queuedMessage.MessageData.MimeMesage.Attachments;
                var reportsDirectory = Directory.CreateDirectory(REPORT_PATH);

                foreach (var attachment in attachments)
                {
                    if (attachment is MimePart part)
                    {
                        var fileName = Path.Combine(REPORT_PATH, part.FileName);

                        using (var stream = File.Create(fileName))
                            part.ContentObject.DecodeTo(stream);
                    }
                }
            });
        }
    }
}