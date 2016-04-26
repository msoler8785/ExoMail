using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface ISaslAuthenticator
    {
        string UserName { get; set; }
        string Password { get; set; }
        bool IsCompleted { get; }
        bool IsInitiator { get; }
        bool IsAuthenticated { get; }
        string SaslMechanism { get; set; }
        //IUserStore UserStore { get; set; }
        int Step { get; set; }
        string GetChallenge();
        void ParseResponse(string response);
        ISaslAuthenticator Create();
    }
}
