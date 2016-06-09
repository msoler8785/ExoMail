using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Protocol;
using ExoMail.Smtp.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.QueueProcessor.Services
{
    public class InboundQueueProcessor : QueueProcessorBase
    {
        public static string InboundQueuePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queues", "Inbound");

        public InboundQueueProcessor()
            : base()
        {
            this.WatchFolder = InboundQueuePath;
        }

        internal override Task ProcessEnvelopeQueue()
        {
            // TODO: Add exceptkon handeling and logging. A lot could go wrong bere.
            return Task.Run(() =>
            {
                while (!Envelopes.IsCompleted)
                {
                    var envelopePath = Envelopes.Take();
                    WaitForFile(envelopePath);

                    var envelope = MessageEnvelope.Load(envelopePath);
                    var outboundEnvelope = new MessageEnvelope() { MessageId = envelope.MessageId };
                    outboundEnvelope.SetSenderDomain(envelope.SenderDomain);
                    outboundEnvelope.SetSenderAddress(envelope.SenderAddress);
                    WaitForFile(envelope.MessagePath);

                    foreach (var recipient in envelope.Recipients)
                    {
                        var user = UserManager.GetUserManager.FindByEmailAddress(recipient.RecipientAddress);

                        if (user != null)
                        {
                            var inbox = Path.Combine(user.MailboxPath, "Inbox");
                            var destination = Path.Combine(inbox, Path.GetFileName(envelope.MessagePath));
                            CreateDirectoryIfNeeded(inbox);

                            if (!File.Exists(destination))
                                File.Copy(envelope.MessagePath, Path.Combine(destination));
                        }
                        else
                        {
                            outboundEnvelope.AddRecipient(recipient.RecipientAddress, recipient.RecipientDomain);
                        }
                    }

                    if (outboundEnvelope.Recipients.Any())
                    {
                        var queuePath = OutboundQueueProcessor.OutboundQueuePath;
                        var destination = Path.Combine(queuePath, Path.GetFileName(envelope.MessagePath));
                        CreateDirectoryIfNeeded(queuePath);

                        outboundEnvelope.SaveEnvelope(destination);

                        //WaitForFile(e.FullPath);
                        File.Copy(envelope.MessagePath, destination);
                    }

                    File.Delete(envelope.MessagePath);
                    File.Delete(envelopePath);
                }
            });
        }

    }
}