using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface IUserIdentity
    {
        string UserId { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string EmailAddress { get; set; }
    }
}
