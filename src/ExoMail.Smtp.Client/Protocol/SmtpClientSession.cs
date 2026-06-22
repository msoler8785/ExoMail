using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpClientSession
    {
        private readonly SmtpClient _smtpClient;
        private string _sender;
        private IEnumerable<string> _recipients;
        private SessionState _sessionState;
        public ServerExtensions ServerExtensions { get; set; }

        public SmtpClientSession(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task Create(string sender, IEnumerable<string> recipients)
        {
            _sender = sender;
            _recipients = recipients;
        }
    }
}