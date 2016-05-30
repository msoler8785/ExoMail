using ExoMail.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Example
{
    public class TestUserStore : IUserStore
    {
        public string Domain
        {
            get
            {
                return "example.net";
            }
        }

        public void AddUser(IUserIdentity userIdentity)
        {
            throw new NotImplementedException();
        }

        public List<IUserIdentity> GetIdentities()
        {
            throw new NotImplementedException();
        }

        public bool IsUserAuthenticated(string userName, string password)
        {
            return userName.ToUpper() == "TUSER" && password == "Str0ngP@$$!!";
        }

        public bool IsValidRecipient(string emailAddress)
        {
            return emailAddress.Contains(this.Domain);
        }
    }
}
