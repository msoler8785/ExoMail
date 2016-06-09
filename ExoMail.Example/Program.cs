using ExoMail.QueueProcessor.Services;
using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Configuration;
using ExoMail.Smtp.Network;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            var appStart = new AppStart(tokenSource.Token);

            appStart.Start();

            do
            {
                Console.WriteLine("Press \"Q\" to quit server.");
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Q);

            tokenSource.Cancel();
        }
    }

    public class AppStart
    {
        private CancellationToken _token { get; set; }

        public AppStart(CancellationToken token)
        {
            this._token = token;
        }

        public void Start()
        {
            Task.Run(() => StartListeningAsync());
            var inboundQueue = new InboundQueueProcessor();
            inboundQueue.Start();

            var outboundQueue = new OutboundQueueProcessor();
            outboundQueue.Start();
        }

        public async Task StartListeningAsync()
        {
            // Build the config.
            var config = MemoryConfig.Create()
                            .WithHostname("exomail01.example.com")
                            .WithPort(2525)
                            .WithServerId("FC30BD4D-1C93-4FBF-BF8F-9788059AF0DC")
                            .WithSessionTimeout(5 * 60 * 1000) // 5 minutes
                            .WithX509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localhost.pfx"), null)
                            //.WithEncryptionRequired()
                            .WithStartTlsSupported()
                            .WithAuthRelayAllowed();

            // Create the MessageStore
            var messageStore = new FileMessageStore();

            // Create the UserStore
            var userStore = new TestUserStore();

            // Create a test userstore repository.
            for (int i = 0; i < 5; i++)
            {
                userStore.AddUser(EmailUser.CreateMailbox("Test", "User0" + i.ToString(), "user0" + i.ToString() + "@example.net"));
            }

            // Add UserStore to the UserManager
            UserManager.GetUserManager.AddUserStore(userStore);

            // Create the server
            SmtpServer server = new SmtpServer(config, messageStore);

            await server.StartAsync(this._token);
        }
    }
}