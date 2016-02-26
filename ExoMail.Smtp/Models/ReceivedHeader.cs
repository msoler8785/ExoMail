using ExoMail.Smtp.Network;
using ExoMail.Smtp.Utilities;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ExoMail.Smtp.Models
{
    public class ReceivedHeader
    {
        private const string DATETIME_FORMAT = "ddd, dd MMM yyyy HH:mm:ss zz00";
        public IPEndPoint LocalEndPoint { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public string ClientHostName { get; set; }
        public string ServerHostName { get; set; }
        public bool IsEncrypted { get; set; }

        public ReceivedHeader()
        {
        }

        public Stream GetReceivedHeaders()
        {
            string security = this.IsEncrypted ? "TLS Encryption" : "Cleartext";
            string remoteHostName = DnsQuery.GetPtrRecord(this.RemoteEndPoint.Address);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Received: from {0} ({1} [{2}]) by", this.ClientHostName, remoteHostName, this.RemoteEndPoint.Address.ToString()));
            sb.AppendLine(String.Format("\t{0} ({1}) with {2}", this.ServerHostName, this.LocalEndPoint.Address.ToString(), security));
            sb.AppendLine(String.Format("\t; {0}", DateTime.Now.ToString(DATETIME_FORMAT)));

            return sb.ToString().ToStream();            
        }
    }
}