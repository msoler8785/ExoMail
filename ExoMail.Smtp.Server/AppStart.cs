using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Network;
using ExoMail.Smtp.Server.Authentication;
using ExoMail.Smtp.Server.IO;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server
{
    public class AppStart
    {
        public static List<SmtpServerFactory> InitializeServers()
        {
            var smtpServers = new List<SmtpServerFactory>();

            //Load the sample certificate
            string certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localhost.pfx");
            var cert = new X509Certificate2(certPath, "");

            //Load the server configs
            List<JsonConfig> configs = JsonConfig.LoadConfigs();

            ////Create the message store
            //IMessageStore messageStore = FileMessageStore.Create
            //    .WithFolderPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Inbound Queue"));

            //Load the user store
            var userStore = JsonUserStore.CreateStore("example.net");
            var authenticators = new List<ISaslAuthenticator>();
            authenticators.Add(new LoginSaslAuthenticator());

            foreach (var config in configs)
            {
                config.X509Certificate2 = cert;
                config.MaxMessageSize = int.MaxValue;
                var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0} Queue", config.ServerType.ToString()));

                var smtpServer = new SmtpServerFactory()
                {
                    ServerConfig = config,
                    MessageStore = FileMessageStore.Create.WithFolderPath(folderPath),
                    UserAuthenticators = authenticators,
                    UserStore = userStore
                };

                smtpServers.Add(smtpServer);
            }
            return smtpServers;
        }
    }
}
