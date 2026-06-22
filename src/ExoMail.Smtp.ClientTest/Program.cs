using ExoMail.Smtp.Client.Network;
using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.ClientTest
{
    public class Program
    {
        static int _threads = 0;
        static int _messagesProcessed = 0;
        private static Stopwatch _sw;

        public static void Main(string[] args)
        {
            Console.WriteLine("Press \"S\" to Start a new sending thread. \"Q\" to Quit.");
            var tcs = new CancellationTokenSource();
            var token = tcs.Token;
            Console.CancelKeyPress += (s, a) => { tcs.Cancel(); };
            _sw = new Stopwatch();
            _sw.Start();
            var timer = new Timer(OnElapsed, null, 0, 1000);
            do
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.S)
                {
                    var task = new App().Start(token, () => Interlocked.Increment(ref _messagesProcessed));
                    Interlocked.Increment(ref _threads);
                    task.ContinueWith(x => Interlocked.Decrement(ref _threads)).ConfigureAwait(false);
                }
            }
            while (!token.IsCancellationRequested);

            _sw.Stop();
            timer.Dispose();
        }

        private static void OnElapsed(object state)
        {
            UpdateTitle();
        }

        private static void UpdateTitle()
        {
            var mps = _messagesProcessed / _sw.Elapsed.TotalSeconds;
            Console.Title = $"Current sending threads: {_threads} Processed: {_messagesProcessed} MPS: {mps}";
        }

        //public static void SetTitle(int threads)
        //{
        //    Console.Title = $"Current sending threads: {threads}";
        //}
    }


    public class App
    {
        const int BATCH_SIZE = 10;
        static int _index = 0;

        public async Task Start(CancellationToken token, Action onMessageProcessed)
        {
            //if (Directory.EnumerateFiles("Emails").Count() - (_index * BATCH_SIZE) <= 0)
            //    _index = 0;

            //var emails = Directory.EnumerateFiles("Emails").Skip(_index * BATCH_SIZE).Take(BATCH_SIZE);
            //Interlocked.Increment(ref _index);


            //var emails = new string[] { @"C:\Users\matt.soler\Source\Repos\ExoMail\src\ExoMail.Smtp.ClientTest\bin\Debug\net5.0\emails\32317.eml" };
            var emails = Directory.EnumerateFiles("Emails", "*.eml").Take(1);
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                using (SmtpClient smtpClient = new SmtpClient(IPAddress.Loopback, "hq-pr-pc12.home"))
                {
                    do
                    {
                        try
                        {
                            await smtpClient.ConnectAsync("Socket", "SenderScore100");
                        }
                        catch
                        {
                            Console.WriteLine("Unable to connect. Trying again in 5 seconds.");
                            Thread.Sleep(5000);
                        }
                    }
                    while (!smtpClient.IsConnected);

                    string[] recipients =
                    {
                            //"nobody@example.com",
                            //"user01@hq-pr-pc01",
                            //"msoler@exorealms.com",
                            //"user00@mattsoler.com",
                            //"user00@kikitsa.com",
                            //"user01@mattsoler.com",
                            //"user01@kikitsa.com",
                            //"user00@mattsoler.net"
                            "test.user@exomail.com"
                        };

                    //while (!token.IsCancellationRequested)
                    //{
                        var envelope = new Envelope[] { new Envelope() };
                        foreach (var item in emails)
                        {
                            using (var fs = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {

                                //long fileLength = 0;
                                //using (var file = File.Open(item, FileMode.Open, FileAccess.Read))
                                //{
                                //    fileLength = file.Length;
                                //}

                                //if (fileLength < 1024 * 1024)

                                try
                                {
                                    await smtpClient.SendAsync("user01@hq-pr-pc01", envelope, fs);
                                    onMessageProcessed.Invoke();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error processing message {item}: {ex.Message}");
                                    throw;
                                }
                                
                            }
                        }
                    //}
                }
                sw.Stop();
                Console.WriteLine($"Client done sending in {sw.Elapsed.TotalSeconds} seconds.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }

    public class Envelope : IMessageEnvelope
    {
        public int DeliveryAttempts { get; set; }

        public bool DeliveryComplete { get; set; }

        public bool DeliveryRetryable { get; set; }

        public Disposition Disposition { get; set; }

        public string Domain { get; set; }

        public string EnvelopePath { get; set; }
        public string LastResponse { get; set; }
        public string MessageId { get; set; }
        public string MessagePath { get; set; }

        public string RemoteIp { get; set; }

        public DateTime NextAttempt { get; set; }

        public DateTime QueueDate { get; set; }

        public DateTime SentDate { get; set; }

        public MailAddress Recipient { get; set; } = MailAddress.Parse("test.user@example.com");

        public MailAddress Sender { get; set; } = MailAddress.Parse("user01@hq-pr-pc01");

        public string SendingUserId { get; set; }
    }

    public static class Extensions
    {
        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static IEnumerable<IEnumerable<T>> Sublist<T>(this IEnumerable<T> source, int numberOfSublists)
        {
            int chunkSize = source.Count() / numberOfSublists;

            return Chunk(source, chunkSize);
        }
    }
}