using ExoMail.Smtp.Server;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Network;
using ExoMail.Smtp.Server.Authentication;
using ExoMail.Smtp.Server.IO;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var tasks = new List<Task>();
            var servers = AppStart.InitializeServers();

            foreach (var item in servers)
            {
                var task = Task.Run(async () =>
                {
                    await item.Start(cancellationToken);
                });
                tasks.Add(task);
            }
            Console.WriteLine("Press any key to stop servers.");
            Console.ReadLine();
            Console.WriteLine("Server is shutting down...");

            try
            {
                foreach (var item in servers)
                {
                    Console.WriteLine("Attempting to stop server on port {0}", item.ServerConfig.Port);
                    cancellationTokenSource.Cancel();
                }

                // Wait 30 seconds for servers to stop or tear down the process.
                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
                foreach(var inner in ex.InnerExceptions)
                {
                    if (inner.Message != null)
                        Console.WriteLine(inner.Message);
                }
            }
        }
    }
}