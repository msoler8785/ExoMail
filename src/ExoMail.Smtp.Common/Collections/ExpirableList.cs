using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ExoMail.Smtp.Common.Collections
{
    public class ExpirableList<T> : List<T> where T : IExpirable
    {
        private readonly Timer _timer;

        public ExpirableList(int dueTime = 0, int interval = 5000)
        {
            _timer = new Timer((s) => CheckExpiration(), null, dueTime, interval);
        }

        private void CheckExpiration()
        {
            var expiredItems = this.Where(x => x.Expired).ToList();

            foreach (var item in expiredItems)
            {
                this.Remove(item);
            }
        }
    }
}
