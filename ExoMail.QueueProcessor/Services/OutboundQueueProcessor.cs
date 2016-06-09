using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using ExoMail.QueueProcessor.Utilities;
using ExoMail.Smtp.Protocol;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.QueueProcessor.Services
{
    public class OutboundQueueProcessor : QueueProcessorBase
    {
        public const int SMTP_SERVER_PORT = 25;
        public static string OutboundQueuePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queues", "Outbound");
        //private static int ConcurrentEnvelopeProcessing = 16;

        public OutboundQueueProcessor()
            : base()
        {
            this.WatchFolder = OutboundQueuePath;
        }

        internal override Task ProcessEnvelopeQueue()
        {
            return Task.Run(async () =>
            {
                while (!Envelopes.IsCompleted)
                {
                    var envelopePath = Envelopes.Take();
                    var envelope = MessageEnvelope.Load(envelopePath);
                    using (var messageStream = new FileStream(envelope.MessagePath, FileMode.Open))
                    {
                        var mimeMessage = MimeMessage.Load(messageStream);
                        foreach (var domain in envelope.RecipientDomains)
                        {
                            var smtpServers = FindSmtpServers(domain);
                            var recipients = envelope
                                            .Recipients
                                            .Where(r => r.RecipientDomain.ToUpper() == domain.ToUpper())
                                            .Select(r => r.RecipientAddress)
                                            .ToList();

                            var isSuccessful = await RelayMessage(mimeMessage, envelope.SenderAddress, recipients, smtpServers);

                            if (isSuccessful)
                            {
                                File.Delete(envelope.MessagePath);
                                File.Delete(envelopePath);
                            }
                            else
                            {

                            }
                        }
                    }

                }

            });
        }

        private IEnumerable<IPAddress> FindSmtpServers(string domain)
        {
            try
            {
                var resolver = new RecursiveDnsResolver();
                var mxRecords = resolver.Resolve<MxRecord>(domain, RecordType.Mx);

                if (mxRecords.Count != 0)
                {
                    var ipAddresses = mxRecords
                        .OrderBy(mx => mx.Preference)
                        .SelectMany(mx => resolver.ResolveHost(mx.Name));

                    return ipAddresses;
                }
                else
                {
                    var aRecord = resolver.ResolveHost(domain);
                    return aRecord;
                }
            }
            catch (Exception)
            {
                return new List<IPAddress>() { IPAddress.Any };
            }
        }

        private Task<bool> RelayMessage(MimeMessage mimeMessage, string senderAddress, List<string> recipients, IEnumerable<IPAddress> smtpServers)
        {
            var task = Task.Run(async () =>
            {
                foreach (var server in smtpServers)
                {
                    SmtpClient client = new SmtpClient();
                    await client.ConnectAsync(server.ToString(), SMTP_SERVER_PORT, false);
                    EventHandler<MailKit.MessageSentEventArgs> messageSent = null;
                    client.MessageSent += messageSent;

                    await client.SendAsync(mimeMessage, new MailboxAddress(null, senderAddress), recipients.Select(x => new MailboxAddress(null, x)));

                    if (messageSent != null)
                        return true;
                }
                return false;
            });

            return task;
        }
    }
}
