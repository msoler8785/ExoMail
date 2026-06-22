using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Docker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<ISmtpServer, SmtpServer>();
                    services.AddSingleton<IVirtualServerRepository, MockVirtualServerRepository>();
                    services.AddSingleton<IMessageStore, BlackholeMessageStore>();
                });
    }
}
