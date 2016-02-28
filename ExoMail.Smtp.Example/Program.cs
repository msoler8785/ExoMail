using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.IO;
using ExoMail.Smtp.Network;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load the sample certificate
            string certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localhost.pfx");
            var cert = new X509Certificate2(certPath, "");

            //Load the server configs
            List<JsonConfig> configs = JsonConfig.LoadConfigs();

            //Create the message store
            IMessageStore messageStore = FileMessageStore.Create
                .WithFolderPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Messages"));

            //Load the user store
            var userStore = JsonUserStore.CreateStore();
            var authenticators = new List<IUserAuthenticator>();
            authenticators.Add(new LoginUserAuthenticator() { UserStore = userStore });

            foreach (var config in configs)
            {
                config.X509Certificate2 = cert;
                config.MaxMessageSize = int.MaxValue;
                SmtpServer smtpServer = new SmtpServer()
                {
                    ServerConfig = config,
                    MessageStore = messageStore,
                    UserAuthenticators = authenticators
                };


                Task.Run(async () => 
                    {
                        try
                        {
                            await smtpServer.Start();
                        }
                        finally
                        {
                            smtpServer.Stop();
                        }
                    });
            }
            Console.ReadLine();
        }
    }
}
