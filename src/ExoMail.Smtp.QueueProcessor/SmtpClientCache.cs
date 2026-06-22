using ExoMail.Smtp.Client.Network;
using ExoMail.Smtp.Common.Collections;
using ExoMail.Smtp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ExoMail.Smtp.QueueProcessor
{
    public class SmtpClientCache : IExpirable
    {
        //private const int MAX_CLIENTS = 4;
        private readonly string _domain;

        private readonly HashSet<CachedSmtpClient> _cachedSmtpClients;
        private readonly ISmtpClientResolver _clientResolver;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly TransformBlock<IQueuedMessage, IQueuedMessage> _queue;
        private readonly IMessageEnvelopeRepository _envelopeRepository;
        private readonly IGatewayOptions _gatewayOptions;
        private readonly Timer _timer;

        private static ExecutionDataflowBlockOptions _blockOptions = new ExecutionDataflowBlockOptions()
        {
            MaxDegreeOfParallelism = -1
        };

        private readonly CancellationTokenSource _tokenSource;

        public bool Expired
        {
            get => _cachedSmtpClients.Count == 0 && _queue.OutputCount == 0;
        }

        public SmtpClientCache(
            string domain,
            ISmtpClientResolver clientResolver,
            IMessageEnvelopeRepository envelopeRepository,
            IGatewayOptions gatewayOptions
            )
        {
            _domain = domain;
            _clientResolver = clientResolver;
            _envelopeRepository = envelopeRepository;
            _gatewayOptions = gatewayOptions;

            _semaphoreSlim = new SemaphoreSlim(1);
            _cachedSmtpClients = new HashSet<CachedSmtpClient>();
            _tokenSource = new CancellationTokenSource();

            _queue = new TransformBlock<IQueuedMessage, IQueuedMessage>(async (message) =>
            {
                await Task.Delay(message.Delay, _tokenSource.Token);
                return message;
            },
            _blockOptions);

            _timer = new Timer(async (x) => await CheckQueues(), null, 10000, 10000);
        }

        public async Task EnqueueMessage(IQueuedMessage queuedMessage)
        {
            await CreateCachedClient();
            await _queue.SendAsync(queuedMessage);
        }

        private async Task CheckQueues()
        {
            if (_semaphoreSlim.CurrentCount == 0)
                return;

            if (_cachedSmtpClients.Where(x => !x.Expired).Count() < _queue.OutputCount)
            {
                await CreateCachedClient();
            }
        }

        private async Task CreateCachedClient()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (_cachedSmtpClients.Where(x => !x.Expired).Count() < _gatewayOptions.MaxClientsPerDomain)
                {
                    SmtpClient smtpClient = await _clientResolver.TryResolveAsync(_domain);

                    if (smtpClient != null)
                    {
                        CachedSmtpClient cachedSmtpClient = new CachedSmtpClient(_domain, _queue, smtpClient, _envelopeRepository, _gatewayOptions);
                        cachedSmtpClient.Initialize();

                        _cachedSmtpClients.Add(cachedSmtpClient);

                        smtpClient.SessionQuitting += (s, e) =>
                        {
                            _cachedSmtpClients.Remove(cachedSmtpClient);
                        };
                    }
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}