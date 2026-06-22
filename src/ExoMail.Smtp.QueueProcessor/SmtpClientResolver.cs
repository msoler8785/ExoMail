using DnsClient;
using ExoMail.Smtp.Client.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public class SmtpClientResolver : ISmtpClientResolver
    {
        private readonly ILogger<SmtpClientResolver> _logger;
        private readonly ILookupClient _lookupClient;
        private readonly ILoggerFactory _loggerFactory;

        public SmtpClientResolver(ILoggerFactory loggerFactory, ILookupClient lookupClient)
        {
            _logger = loggerFactory.CreateLogger<SmtpClientResolver>();
            _loggerFactory = loggerFactory;
            _lookupClient = lookupClient;
        }

        public async Task<SmtpClient> TryResolveAsync(string domain)
        {
            IDnsQueryResponse response = await _lookupClient.QueryAsync(domain, QueryType.MX);
            var mxRecords = response.Answers.MxRecords().OrderByDescending(x => x.Preference);
            SmtpClient smtpClient = null;

            foreach (var mx in mxRecords)
            {
                smtpClient = await TryConnectToARecordAsync(mx.Exchange.ToString());

                if (smtpClient != null)
                {
                    _logger.LogInformation("SmtpClient connection established at {0} for {1}", mx.Exchange, domain);
                    break;
                }
            }

            if (smtpClient == null)
            {
                smtpClient = await TryConnectToARecordAsync(domain);
            }

            return smtpClient;
        }

        private async Task<SmtpClient> TryConnectToARecordAsync(string aRecord)
        {
            IDnsQueryResponse response = await _lookupClient.QueryAsync(aRecord, QueryType.A);
            var aRecords = response.Answers.ARecords();
            SmtpClient smtpClient = null;

            foreach (var a in aRecords)
            {
                SmtpClient client = new SmtpClient(a.Address, Environment.MachineName, loggerFactory: _loggerFactory);

                try
                {
                    _logger.LogInformation("SmtpClient connecting to {0}", aRecord);
                    await client.ConnectAsync();

                    if (client.IsConnected)
                    {
                        smtpClient = client;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.HResult, ex, "Connecting to {0} failed.", a.Address);
                }
            }

            return smtpClient;
        }
    }
}