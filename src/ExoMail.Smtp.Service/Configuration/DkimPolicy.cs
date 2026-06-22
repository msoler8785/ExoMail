using ExoMail.Smtp.Common.Interfaces;
using MimeKit;
using MimeKit.Cryptography;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExoMail.Smtp.Service
{
    public class DkimPolicy : IDkimPolicy
    {
        public bool UseDkim { get; private set; }

        public string Domain { get; private set; }

        public string Selector { get; private set; }

        public string[] Headers { get; private set; }

        [JsonIgnore]
        public DkimSigner DkimSigner { get; private set; }

        [JsonProperty(PropertyName = "PrivateKey")]
        private string _privateKey;

        public DkimPolicy(string domain, string selector, string[] headers, string privateKey, bool useDkim = true)
        {
            _privateKey = privateKey;
            Domain = domain;
            Selector = selector;
            Headers = headers;
            UseDkim = useDkim;
            DkimSigner = Build(_privateKey, domain, selector);
        }

        //public static IDkimPolicy Create(string domain, string selector, string[] headers, string key, bool useDkim = true)
        //{
        //    var policy = new DkimPolicy(key,domain,selector)
        //    {
        //        UseDkim = useDkim,
        //        Headers = headers,
        //    };

        //    return policy;
        //}

        private static string[] _delims = new string[] { "\r\n-", "-\r\n", "\n-", "-\n" };

        private DkimSigner Build(string dkimKey, string domain, string selector)
        {
            //var keyParts = dkimKey.Split(_delims, StringSplitOptions.RemoveEmptyEntries);
            //dkimKey = keyParts.Length == 1 ? dkimKey : keyParts[1];


            //var keyBytes = Convert.FromBase64String(dkimKey);
            //var key = PrivateKeyFactory.CreateKey(keyBytes);

            DkimSigner dkimSigner;

            var keyBytes = Encoding.ASCII.GetBytes(dkimKey);
            using (var stream = new MemoryStream())
            {
                stream.Write(keyBytes, 0, keyBytes.Length);
                stream.Position = 0;
                dkimSigner = new DkimSigner(stream, domain, selector);
            }

            return dkimSigner;
        }
    }
}