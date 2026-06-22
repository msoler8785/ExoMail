using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ExoMail.Smtp.Common.Collections
{
    public class ExpirableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : IExpirable
    {
        private readonly ILogger _logger;
        private readonly Timer _timer;

        public ExpirableDictionary(ILoggerFactory loggerFactory = null, int dueTime = 0, int interval = 5000)
            : this(EqualityComparer<TKey>.Default, loggerFactory, dueTime, interval)
        {
        }

        public ExpirableDictionary(IEqualityComparer<TKey> comparer, ILoggerFactory loggerFactory = null, int dueTime = 0, int interval = 5000)
            : base(comparer)
        {
            _logger = loggerFactory?.CreateLogger<ExpirableDictionary<TKey, TValue>>();
            _timer = new Timer((s) => CheckExpiration(), null, dueTime, interval);
        }

        private void CheckExpiration()
        {
            var expiredItems = this.Where(x => x.Value.Expired).ToList();

            foreach (var item in expiredItems)
            {
                _logger?.LogInformation("Removing expired item from Dictionary with key: {0}", item.Key);
                this.Remove(item.Key);
            }
        }
    }
}