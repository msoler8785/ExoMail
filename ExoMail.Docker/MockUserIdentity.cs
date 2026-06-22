using ExoMail.Smtp.Common.Interfaces;
using System.Collections.Generic;

namespace ExoMail.Docker
{
    internal class MockUserIdentity : IUserIdentity
    {
        public MockUserIdentity()
        {
            EmailAddresses = new List<string>() { "test.user@example.com" };
        }

        public string UserId => "42";

        public string UserName => "test.user";

        public List<string> EmailAddresses { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string MailboxPath => throw new System.NotImplementedException();

        public long MaxMessageSize => 50 * 1024 * 1024; //50 MB

        public long MaxMailboxSize => long.MaxValue;

        public long MailboxSize => 0;

        public long MessageCount => 0;

        public bool IsActive => true;

        public List<string> Folders => throw new System.NotImplementedException();

        public string VirtualServerId => "1000";

        public void SetPassword(string password)
        {
            throw new System.NotImplementedException();
        }

        public bool VerifyPassword(string password)
        {
            return true;
        }
    }
}