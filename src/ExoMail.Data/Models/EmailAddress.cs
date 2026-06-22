using System;

namespace ExoMail.Data.Models
{
    public class EmailAddress
    {
        public int EmailAddressId { get; set; }
        public string Address { get; set; }

        public Guid UserIdentityId { get; set; }
        public virtual UserIdentity UserIdentity { get; set; }
    }
}