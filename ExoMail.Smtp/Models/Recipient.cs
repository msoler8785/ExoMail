using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Models
{
    public class Recipient
    {
        public string RecipientAddress { get; set; }
        public string RecipientDomain { get; set; }
    }
}
