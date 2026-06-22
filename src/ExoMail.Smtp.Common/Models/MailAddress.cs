using System;
using System.Linq;

namespace ExoMail.Smtp.Common.Models
{
    public class MailAddress
    {
        private static char[] _split = new char[] { '<', '>' };
        public string LocalPart { get; set; }
        public string Domain { get; set; }
        public string FullAddress { get; set; }

        public static MailAddress Parse(string address)
        {
            MailAddress mailAddress;

            TryParse(address, false, out mailAddress);

            return mailAddress;
        }

        public static bool TryParse(string address, bool allowNullAddress, out MailAddress mailAddress)
        {
            mailAddress = new MailAddress();

            if (address.Equals("<>"))
                return allowNullAddress;

            try
            {
                string fullAddress = address.Split(_split, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                int at = fullAddress.LastIndexOf('@');

                string localPart = fullAddress.Substring(0, at);
                string domain = fullAddress.Substring(at + 1, fullAddress.Length - localPart.Length - 1);

                mailAddress.Domain = domain;
                mailAddress.FullAddress = fullAddress;
                mailAddress.LocalPart = localPart;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            MailAddress address = obj as MailAddress;

            return this.FullAddress.Equals(address.FullAddress, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.FullAddress.ToLower().GetHashCode();
        }
    }
}