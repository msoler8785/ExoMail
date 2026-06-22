using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExoMail.Smtp.Common.Extensions
{
    public class SmtpCancellationTokenSource : CancellationTokenSource
    {
        public string CancellationReason { get; private set; }

        public SmtpCancellationTokenSource()
            : base()
        {
        }

        public void Cancel(string cancellationReason)
        {
            CancellationReason = cancellationReason;
            base.Cancel();
        }
    }
}
