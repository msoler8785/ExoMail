using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Utilities;
using ExoMail.Smtp.Service.Configuration;
using ExoMail.Smtp.Service.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service.Services
{
    public class SmtpServerRepository : JsonRepository<ISmtpServerConfig>, ISmtpServerRepository
    {
        public bool IsStarted { get; private set; } = false;
        private List<Task> _tasks;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _tokenSource;
        private TaskCompletionSource<bool> _taskSource;
        internal override string ConfigName => "smtp-servers.json";

        internal override void OnConfigReloaded()
        {
            if (!IsStarted)
            {
                return;
            }
            else if (_taskSource?.Task == null && _taskSource.Task.IsCompleted)
            {
                return;
            }
            else if (!_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = new CancellationTokenSource();
            }

            if (Interop.IsWindows())
            {
                try
                {
                    IsStarted = false;
                    Task.WaitAll(_tasks.ToArray());
                }
                catch (AggregateException ex)
                {
                    _logger.LogDebug(ex.HResult, ex, ex.Message);
                }

                _tasks.Clear();
                LoadServers();
            }
            else
            {
                // Autoreload is not working on Linux right now so we stop the server.
                // systemd will reload the service on shutdown.
                Stop().Wait();
            }
        }

        public SmtpServerRepository(
            ILoggerFactory loggerFactory,
            IOptions<RepositoryOptions> options,
            DependencyContractResolver contractResolver,
            IServiceProvider serviceProvider
            )
            : base(loggerFactory, options, contractResolver)
        {
            _tokenSource = new CancellationTokenSource();
            _tasks = new List<Task>();
            _serviceProvider = serviceProvider;
        }

        public Task Start()
        {
            ReadConfig();
            LoadServers();
            _taskSource = new TaskCompletionSource<bool>();
            return _taskSource.Task;
        }

        public async Task Stop()
        {
            IsStarted = false;
            _tokenSource.Cancel();

            await Task.WhenAll(_tasks).ContinueWith(task =>
            {
                _taskSource.SetResult(true);
            });
        }

        private void LoadServers()
        {
            foreach (var item in Items)
            {
                var server = _serviceProvider.GetRequiredService<ISmtpServer>();
                var task = server.StartAsync(item, _tokenSource.Token);
                _tasks.Add(task);
            }
            IsStarted = true;
        }
    }
}