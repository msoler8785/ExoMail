using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Authentication
{
    public abstract class SaslMechanismBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SaslMechanism { get; set; }
        public bool IsCompleted { get; private set; }
        public bool IsInitiator { get; private set; }
        public int Step { get; set; }

        public SaslMechanismBase()
        {
            this.IsCompleted = false;
        }

        protected void SetCompleted(bool isCompleted)
        {
            this.IsCompleted = isCompleted;
        }

        protected void SetInitiator(bool isInitiator)
        {
            this.IsInitiator = isInitiator;
        }
    }
}
