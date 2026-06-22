using ExoMail.Data.Models;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ExoMail.Smtp.Service
{
    public class EmailUser : IUserIdentity
    {
        private static SHA256 _sha256 = SHA256.Create();
        public List<string> EmailAddresses { get; set; }
        public string FirstName { get; set; }

        [JsonIgnore]
        public List<string> Folders
        {
            get
            {
                if (Directory.Exists(MailboxPath))
                    return Directory.GetDirectories(MailboxPath).ToList();

                return new List<string>();
            }
        }

        public bool IsActive { get; set; }
        public string LastName { get; set; }
        public string MailboxPath { get; set; }
        public long MailboxSize { get; set; }
        public long MaxMailboxSize { get; set; }
        public long MaxMessageSize { get; set; }

        [JsonIgnore]
        public long MessageCount
        {
            get
            {
                if (Directory.Exists(MailboxPath))
                    return Directory.EnumerateFiles(MailboxPath).Count();

                return 0;
            }
        }

        [JsonProperty(PropertyName ="Password")]
        private string _password;

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string VirtualServerId { get; set; }
       
        public EmailUser()
        {
            UserName = string.Empty;
            EmailAddresses = new List<string>();
        }

        public static EmailUser CreateFromConfig(UserIdentity userIdentity)
        {
            var user = new EmailUser()
            {
                FirstName = userIdentity.FirstName,
                LastName = userIdentity.LastName,
                EmailAddresses = userIdentity.EmailAddresses.Select(x => x.Address).ToList(),
                IsActive = userIdentity.IsActive,
                MailboxPath = userIdentity.MailboxPath,
                UserId = userIdentity.UserIdentityId.ToString(),
                UserName = userIdentity.UserName,
                _password = userIdentity.Password,
                MailboxSize = userIdentity.MailboxSize,
                MaxMailboxSize = userIdentity.MaxMailboxSize,
                MaxMessageSize = userIdentity.MaxMessageSize,
                VirtualServerId = userIdentity.VirtualServerConfigId.ToString()
            };

            return user;
        }

        public override int GetHashCode()
        {
            return UserId?.GetHashCode() ?? base.GetHashCode();
        }

        public string HashPassword(string password)
        {
            string salted = $"{UserId}{password}";
            byte[] saltedBytes = Encoding.UTF8.GetBytes(salted);
            byte[] hash = _sha256.ComputeHash(saltedBytes);
            string encodedHash = Convert.ToBase64String(hash);

            return encodedHash;
        }

        public bool VerifyPassword(string password)
        {
            string hashed = HashPassword(password);
            return hashed == _password;
        }

        public void SetPassword(string password)
        {
            _password = HashPassword(password);
        }

        public override string ToString()
        {
            return $"Name: {FirstName} {LastName} UserName: {UserName} UserId: {UserId}";
        }
    }
}