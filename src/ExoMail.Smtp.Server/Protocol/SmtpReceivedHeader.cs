using ExoMail.Smtp.Common.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpReceivedHeader
    {
        private const string DATETIME_FORMAT = "ddd, dd MMM yyyy HH:mm:ss zz00";
        private readonly SmtpSession _smtpSession;
        private readonly string _messageId;

        public SmtpReceivedHeader(SmtpSession smtpSession, string messageId)
        {
            _smtpSession = smtpSession;
            _messageId = messageId;
        }

        public string GetReceivedHeaders()
        {
            string received = $"from {_smtpSession.SessionNetwork.RemoteDomainName} ({_smtpSession.SessionNetwork.RemotePtr} [{_smtpSession.SessionNetwork.RemoteEndPoint.Address}]) by {_smtpSession._serverConfig.HostName} with ESMTPS id {_messageId} ";
            var sb = new StringBuilder(received);

            if (_smtpSession.SessionNetwork.IsEncrypted)
            {
                sb.Append($"(version={_smtpSession.SessionNetwork.SslStream.SslProtocol.ToString()} cipher={_smtpSession.SessionNetwork.SslStream.CipherAlgorithm}); ");
            }
            else
            {
                sb.Append($"(Cleartext); ");
            }

            //string ptrRecord = await _smtpSession.SessionNetwork.PtrRecordAsync;


            sb.Append(DateTime.Now.ToString(DATETIME_FORMAT));
                  

            return sb.ToString();
        }
    }
}