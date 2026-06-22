using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Utilities
{
    public class SessionTimer
    {
        public TimeSpan Timeout { get; set; }
        private Stopwatch _stopWatch { get; set; }

        public delegate void TimeoutHandler(object sessionTimer, EventArgs args);

        public event TimeoutHandler TimeoutElapsed;

        public SessionTimer(TimeSpan timeout)
        {
            Timeout = timeout;
            _stopWatch = new Stopwatch();
        }

        protected void OnTimeoutElapsed(object sessionTimer, EventArgs args)
        {
            if (_stopWatch.IsRunning)
                TimeoutElapsed?.Invoke(sessionTimer, args);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                _stopWatch.Start();
                while (_stopWatch.IsRunning && _stopWatch.Elapsed < Timeout)
                {
                    await Task.Delay(1000);
                }
                OnTimeoutElapsed(this, new EventArgs());
            });
        }

        public void Stop()
        {
            _stopWatch.Reset();
        }

        public void Restart()
        {
            _stopWatch.Restart();
        }
    }
}