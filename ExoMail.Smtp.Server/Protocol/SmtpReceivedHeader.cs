using ExoMail.Smtp.Extensions;
using ExoMail.Smtp.Server.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpReceivedHeader
    {
        private const string DATETIME_FORMAT = "ddd, dd MMM yyyy HH:mm:ss zz00";
        private SmtpSession _smtpSession { get; set; }

        public SmtpReceivedHeader(SmtpSession smtpSession)
        {
            _smtpSession = smtpSession;
        }

        public async Task<Stream> GetReceivedHeaders()
        {
            string security = _smtpSession.IsEncrypted ? "TLS Encryption" : "Cleartext";

            string ptrRecord = await _smtpSession.SessionNetwork.PtrRecordAsync;

            var receievedHeader = String.Format("Received: from {0} ({1} [{2}]) by {3} ({4}) with {5}; {6}",
                  _smtpSession.SessionNetwork.RemoteDomainName.ToString().TrimEnd('.'),
                  ptrRecord,
                  _smtpSession.SessionNetwork.RemoteEndPoint.Address.ToString(),
                  _smtpSession.ServerConfig.HostName,
                  _smtpSession.SessionNetwork.ServerIpAdderss.ToString(),
                  security,
                  DateTime.Now.ToString(DATETIME_FORMAT)
                  );

            return receievedHeader.WordWrap(78, "\t").ToStream();
        }
    }
}