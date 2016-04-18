using ExoMail.Smtp.Network;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ExoMail.Smtp.Tasks
{
    /// <summary>
    /// Singleton SmtpSessionManager.
    /// </summary>
    public sealed class SmtpSessionManager
    {
        private static readonly Lazy<SmtpSessionManager> _smtpSessionManager =
            new Lazy<SmtpSessionManager>(() => new SmtpSessionManager());

        public static SmtpSessionManager GetSmtpSessionManager { get { return _smtpSessionManager.Value; } }

        private SmtpSessionManager()
        {
            this.SmtpSessions = new List<SmtpSessionBase>();
        }

        public List<SmtpSessionBase> SmtpSessions { get; set; }

        public static void Add(SmtpSessionBase smtpSession)
        {
            GetSmtpSessionManager.SmtpSessions.Add(smtpSession);
        }

        public static void Remove(SmtpSessionBase smtpSession)
        {
            GetSmtpSessionManager.SmtpSessions.Remove(smtpSession);
        }

        public static void StopAllSessions()
        {
            GetSmtpSessionManager.SmtpSessions.ForEach(x => x.CancellationTokenSource.Cancel());
        }

        private static void IdleTimeout(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}