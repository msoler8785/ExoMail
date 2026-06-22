using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Docker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISmtpServer _smtpServer;

        public Worker(
            ILogger<Worker> logger,
            ISmtpServer smtpServer
            )
        {
            _logger = logger;
            _smtpServer = smtpServer;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}

            var config = MemoryConfig.Create();

            await _smtpServer.StartAsync(config, token);
        }
    }
}
