using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Protocol;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Docker
{
    public class BlackholeMessageStore : IMessageStore
    {
        private readonly ILogger<BlackholeMessageStore> _logger;

        public BlackholeMessageStore(ILogger<BlackholeMessageStore> logger)
        {
            _logger = logger;
        }

        public async Task<string> Save(SmtpSession smtpSession, Stream message)
        {
            _logger.LogInformation("Saving message FROM {0}", smtpSession.SmtpStateTable.Sender.FullAddress);
            message.Position = 0;

            var mime = await MimeMessage.LoadAsync(message, true);

            _logger.LogInformation("Message parsed: {0}", mime.Subject);

            return "250 Message Blackholed";
        }
    }
}
