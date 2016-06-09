using ExoMail.Smtp.Authentication;
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
        private List<IUserIdentity> _users { get; set; }

        public TestUserStore()
        {
            this._users = new List<IUserIdentity>();
        }

        public string Domain
        {
            get
            {
                return "example.net";
            }
        }

        public void AddUser(IUserIdentity userIdentity)
        {
            this._users.Add(userIdentity);
        }

        public List<IUserIdentity> GetIdentities()
        {
            return this._users;
        }

        public bool IsUserAuthenticated(string userName, string password)
        {
            userName = userName.ToUpper();
            var user = this._users.FirstOrDefault(u => u.UserName.ToUpper() == userName);
            if (user == null)
                return false;

            // In a real world implementation this would compare a hashed version of the password.
            return user.Password == password;
        }

        public bool IsValidRecipient(string emailAddress)
        {
            emailAddress = emailAddress.ToUpper();

            return _users.Any(u => u.EmailAddress.ToUpper() == emailAddress);
        }
    }
}
