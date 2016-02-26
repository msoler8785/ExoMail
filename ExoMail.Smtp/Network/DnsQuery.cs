using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ARSoft.Tools.Net;
using ARSoft;
using ARSoft.Tools.Net.Dns;
using System.Net.Sockets;

namespace ExoMail.Smtp.Network
{
    public class DnsQuery
    {
        public static async Task<string> GetPtrRecordAsync(IPAddress ipAddress)
        {
            var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
            return hostEntry.HostName;
        }

        public static string GetPtrRecord(IPAddress ipAddress)
        {
            var hostEntry = Dns.GetHostEntry(ipAddress);
            return hostEntry.HostName;
        }

        public static async Task<string> GetARecord(string hostName)
        {
            var aRecord = await Dns.GetHostEntryAsync(hostName);
            return aRecord.AddressList.FirstOrDefault().ToString();
        }

        public static async Task<TxtRecord> GetSpfRecord(string hostName)
        {
            DnsMessage dnsMessage = await DnsClient.Default.ResolveAsync(DomainName.Parse(hostName), RecordType.Txt);

            TxtRecord txtRecord = dnsMessage
                .AnswerRecords
                .OfType<TxtRecord>()
                .FirstOrDefault(x => x.TextData.ToLower().Contains("v=spf1"));

            return txtRecord;
        }

        public static async Task<List<MxRecord>> GetMxRecords(DomainName domainName)
        {
            DnsMessage dnsMessage = await DnsClient.Default.ResolveAsync(domainName, RecordType.Mx);
            List<MxRecord> mxRecords = dnsMessage.AnswerRecords.OfType<MxRecord>().ToList();
            return mxRecords;
        }

        /// <summary>
        /// Checks an IpAddress or Hostname to see if it is listed on the configure DNSBL.
        /// </summary>
        /// <param name="hostNameOrIpAddress">A valid hostname or IpAddress of the server to lookup.</param>
        /// <returns>Returns true for positive listing in DNSBL.</returns>
        public static async Task<bool> IsOnDnsblAsync(string hostNameOrIpAddress)
        {
            //var queryIp = await Dns.GetHostEntryAsync(hostNameOrIpAddress);
            //if (queryIp == null) return false;
            //DnsMessage dnsMessage;

            //foreach (var address in queryIp.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork))
            //{
            //    // Reverse the order of the octets and append the DNSBL zone.
            //    //var lookup = address.GetReverseLookupAddress()
            //    //    .ToString()
            //    //    .Split('.')
            //    //    .Reverse()
            //    //    .Aggregate((i, j) => i + "." + j) + "." + SmtpServer.Core.ServerConfig.DnsblProvider;

            //    var lookup = address.Reverse().ToString() + "." + SmtpServer.Core.ServerConfig.DnsblProvider;

            //    // Query DNSBL Zone for any A records.
            //    dnsMessage = await DnsClient.Default.ResolveAsync(DomainName.Parse(lookup), RecordType.A);
                
            //    // TODO: Presence of any A records usually indicates positive listing in DNSBL.
            //    // Return records can be used to provide additional diagnostic info.
            //    if (dnsMessage.AnswerRecords.OfType<ARecord>().Any()) return true;
            //}
            return false;
        }
    }
}
