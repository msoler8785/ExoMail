using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Utilities;
using ExoMail.Smtp.QueueProcessor;
using ExoMail.Smtp.Service.Configuration;
using ExoMail.Smtp.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Any())
            {
                CreateUsers(args);
                return;
            }

            var service = new Service();

            AssemblyLoadContext.Default.Unloading += async context =>
            {
                Console.WriteLine("Service is unloading.");

                if (service.IsRunning)
                    await service.Stop();
            };

            if (Environment.UserInteractive)
            {
                Task.Run(async () =>
                {
                    do
                    {
                        Console.WriteLine("Press \"Q\" to quit server.");
                    }
                    while (Console.ReadKey(true).Key != ConsoleKey.Q);

                    if (service.IsRunning)
                        await service.Stop();
                })
                .RunAsync();
            }

            try
            {
                service.Start().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    Console.WriteLine(inner.Message);
                    Console.WriteLine(inner.StackTrace);
                }
                Environment.ExitCode = ex.HResult;
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        //private static void StartService(Service service)
        //{
        //    try
        //    {
        //        service.Start().Wait();
        //    }
        //    catch (AggregateException ex)
        //    {
        //        foreach (var inner in ex.InnerExceptions)
        //        {
        //            Console.WriteLine(inner.Message);
        //            Console.WriteLine(inner.StackTrace);
        //        }
        //        Environment.ExitCode = ex.HResult;
        //    }
        //}

        private static void CreateUsers(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            IServiceProvider provider = new ServiceCollection()
                .AddSingleton<DependencyContractResolver>()
                .AddSingleton<IVirtualServerRepository, JsonVirtualServerRepository>()
                .AddSingleton<ISmtpServerRepository, SmtpServerRepository>()
                .AddLogging(x => x.AddConsole().AddDebug())
                .AddResolver()
                .Configure<RepositoryOptions>(configuration.GetSection("RepositoryOptions"))
                .BuildServiceProvider();
            IVirtualServerRepository serverRepository = provider.GetRequiredService<IVirtualServerRepository>();
            ISmtpServerRepository smtpServerRepository = provider.GetRequiredService<ISmtpServerRepository>();

            foreach (var domain in args)
            {
                IGatewayOptions serverOptions = GatewayOptions.Default();
                IVirtualServer virtualServer = new VirtualServer()
                {
                    Active = true,
                    Description = "The default virtual server.",
                    Name = "Default Virtual Server",
                    VirtualServerId = Guid.NewGuid().ToString(),
                };

                IMessageGateway gateway = new MxMessageGateway(null, null, provider.GetRequiredService<ILoggerFactory>())
                {
                    GatewayOptions = GatewayOptions.Default()
                };

                IMessageRoute route = new MessageRoute()
                {
                    Metric = 10,
                    MessageRouteId = Guid.NewGuid().ToString(),
                    VirtualServerId = virtualServer.VirtualServerId,
                    MessageGateway = gateway,
                };

                string[] headers = { "from", "to", "message-id", "mime-version", "subject" };
                string key = File.ReadAllText("dkim-key.crt");

                IDkimPolicy policy = new DkimPolicy(domain, "key01", headers, key);

                for (int i = 0; i < 2; i++)
                {
                    string address = $"user0{i}@{domain}";
                    var user = new EmailUser()
                    {
                        UserId = Guid.NewGuid().ToString(),
                        FirstName = "Test",
                        LastName = "User",
                        IsActive = true,
                        UserName = address,
                        MailboxPath = Path.Combine("Users", address),
                        VirtualServerId = virtualServer.VirtualServerId,
                    };
                    user.SetPassword("ChangeMe!");
                    user.EmailAddresses.Add(address);
                    virtualServer.Users.Add(user);
                }
                virtualServer.Routes.Add(route);
                virtualServer.DkimPolicies.Add(policy);
                virtualServer.Options.Add("TrackingDomain", "tracking.example.com");
                serverRepository.Create(virtualServer);
            }
            serverRepository.Create(new SystemVirtualServer());
            serverRepository.SaveChanges();

            foreach (var config in SmtpServerConfig.GetDefaults())
            {
                smtpServerRepository.Create(config);
            }
            smtpServerRepository.SaveChanges();
            Environment.Exit(0);
            return;
        }
    }
}