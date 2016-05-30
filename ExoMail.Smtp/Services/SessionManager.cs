using ExoMail.Smtp.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Services
{
    /// <summary>
    /// A singleton class for storing information about active sessions.
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// A lazy singleton initializer.
        /// </summary>
        private static readonly Lazy<SessionManager> _instance =
            new Lazy<SessionManager>(() => new SessionManager());

        /// <summary>
        /// Retrieve the singleton instance.
        /// </summary>
        public static SessionManager GetSessionManager { get { return _instance.Value; } }

        /// <summary>
        /// Private constructor for this singleton.
        /// </summary>
        private SessionManager()
        {
            this.SmtpSessions = new List<SmtpSession>();
        }

        /// <summary>
        /// A list of active SmtpSessions.
        /// </summary>
        public List<SmtpSession> SmtpSessions { get; set; }

        /// <summary>
        /// Call stop method on all active sessions.
        /// </summary>
        public void StopSessions()
        {
            foreach(var session in this.SmtpSessions)
            {
                session.StopSession();
            }
        }
    }
}
