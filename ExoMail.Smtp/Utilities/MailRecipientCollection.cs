using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using ExoMail.Smtp.Models;
using ExoMail.Smtp.Network;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Utilities
{
    public class MailRecipientCollection
    {
        public DomainName DomainName { get; set; }
        public List<MailboxAddress> Recipients { get; set; }

        public MailRecipientCollection()
        {
            this.Recipients = new List<MailboxAddress>();
        }

        public List<MailRecipientCollection> GetRecipientCollections(List<SmtpCommand> smtpCommands)
        {
            var mailRecipientCollections = new List<MailRecipientCollection>();
            smtpCommands = smtpCommands.Where(x => x.Arguments.Any(a => a.Contains("TO:"))).ToList();

            var domainGroups = smtpCommands.GroupBy(x => 
                GetDomainFromAddress(
                    Regex.Match(x.Arguments.ElementAtOrDefault(0), "<(.*?)>").Value
                    )).ToList();

            foreach(var domain in domainGroups)
            {
                var mailRecipentCollection = new MailRecipientCollection()
                {
                    DomainName = domain.Key
                };

                foreach (var smtpCommand in domain)
                {
                    MailboxAddress mailboxAddress = null;
                    var addressString = Regex.Match(smtpCommand.Arguments.FirstOrDefault(), "<(.*?)>").Value;
                    var isMailboxAddress = MailboxAddress.TryParse(addressString, out mailboxAddress);

                    if (isMailboxAddress)
                    {
                        mailRecipentCollection.Recipients.Add(mailboxAddress);
                    }
                }

                mailRecipientCollections.Add(mailRecipentCollection);
            }
            return mailRecipientCollections;
        }

        public async Task<List<MxRecord>> GetMxRecords()
        {
            return await DnsQuery.GetMxRecords(this.DomainName);
        }

        private DomainName GetDomainFromAddress(string address)
        {
            var domain = new MailAddress(address).Host;
            return DomainName.Parse(domain);
        }
    }
}
