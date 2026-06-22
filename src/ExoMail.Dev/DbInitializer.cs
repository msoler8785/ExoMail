using ExoMail.Data;
using ExoMail.Data.Models;
using ExoMail.Smtp.QueueProcessor;
using ExoMail.Smtp.Service;
using ExoMail.Smtp.Service.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ExoMail.Dev
{
    public class DbInitializer
    {
        private readonly ConfigContext _configContext;

        public DbInitializer(ConfigContext configContext)
        {
            _configContext = configContext;
        }

        public void SeedDatabase()
        {
            _configContext.Database.Migrate();

            if (!_configContext.VirtualServerConfigs.Any())
            {
                var virtualServerConfig = new VirtualServerConfig()
                {
                    Description = "The default virtual server.",
                    IsActive = true,
                    Name = "Default"
                };

                var emailUser = new EmailUser();
                var id = Guid.NewGuid();
                emailUser.UserId = id.ToString();

                var password = emailUser.HashPassword("password");

                var user = new UserIdentity()
                {
                    UserIdentityId = id,
                    FirstName = "Test",
                    LastName = "User",
                    IsActive = true,
                    MailboxPath = @"Users\test.user@example.com",
                    MaxMailboxSize = 0,
                    MaxMessageSize = 25600000,
                    UserName = "test.user@example.com",
                    Password = password
                };

                var emailAddress = new EmailAddress()
                {
                    Address = "test.user@example.com"
                };

                user.EmailAddresses.Add(emailAddress);

                //var mxRoute = new MxMessageRouteConfig()
                //{
                //    Destination = "*",
                //    Metric = 100,
                //    TypeName = typeof(MxMessageGateway).FullName,
                //    MaxClientsPerDomain = 4,
                //    MaxMessageRetries = 20,
                //    MaxMessagesPerSession = 100,
                //    MessageDeferralTime = 300
                //};

                //var nullRoute = new NullMessageRouteConfig()
                //{
                //    Destination = "gmail.com",
                //    Metric = 120,
                //    TypeName = typeof(NullMessageGateway).FullName
                //};

                var dkimPolicyConfig = new DkimPolicyConfig()
                {
                    Domain = "google.com",
                    Headers = "subject;date;from;to;mime-version;",
                    PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\r\nMIIEogIBAAKCAQEAvElcwgRUhOsjvEGUu+cdkipk/72ftAwMbClPSDaXNm+GX3YD\r\n/Et2uTMOwm7Q55UgbGPahTs3QDeOvOqk39ZbDFSMOlm2J7P+44x3L1lMJP0eTTrp\r\nVu0js/Br8Pj1pFbbLHkL+YMOdLISctQuGZczHMjKLMZK6daNG10hLPY60Xwwphv4\r\nbyyYOGNcSuFFUdL+xOF4whhyp7wc9xjwPGtyVokraqNpjaJOlnJT0bWrVETtgwcg\r\nCX9BeG9f4e6M1Mt5p8V2Yd1iWT8XJhZvzAjnGSpyuKW1yKlL7Gn8l7iR0rl4phNJ\r\ngRH2j0Jw3sXmfhomGm62O+ME7w3GwMVAyp3YVQIDAQABAoIBAQCv3SILgfnjsLFP\r\nHD4tB+DB7mEp0dbCV+KegDMyMvMoACEVNujgFShOMVde2hy7wbloJBguCzI/A+WJ\r\npOzcuZ2PNcVIlxShnui7Zy7HhfOuNNHBtv3NmG+ZmT1o9VCS8CGFN4asxfGRo+XB\r\nZVetxWETSKQ2D/xWz3Q7E9Bx64bitC+mDSBWmObV8ZIn/RL3dE2EQifZ1tpgwl8Q\r\nFzqU+jnOoV1MyLCI2fMInMTGJBwp1SYsBZA2wM1wUocnrOXF1YRiBsyh4P4hZWvl\r\nUtD4aav/HAmRu44OgG9JnhS8jij74RSXMU13DDgCbvncskkUiFfAeVBAnzXwO9M0\r\n9G35jBW9AoGBANxOoxN8KB4jrj4fkaJGOvXllsIoxNbfSPguf0C8PUvGjiMQARaT\r\n11HV2gtC6ELNp2+NEDf5NBekyyEyxxQ1nDILTnOxoHKBglK3MxfD3xgh6HDxjsp4\r\nyj41KrPx+fnP/OzSF8khKbEkor0DNwQz6Nqj5u1vcTAQ8AUYdQv2U80bAoGBANrK\r\npySLMgXDqAXxqwD6RkypekwFmDHBYP1nIGFhzm1R/hX7ohSJ5OZzb4O36+P3Kb5Y\r\nAtumWRKCyfDDGkh4U9Fn0MB3R3aWxSFddUxvFSJxHLD1j4pstHfLhlYj9hWFI4zA\r\nTDJlx8Noj+a69Tm54wVkc2oXM8dyA36/qeEv3ndPAoGBAMvrZftFMwUi1cffBLiF\r\n9VFioDc78WdK6on+z/8lvj8DnNFLoRY65lVzcw51cfNT1KXmlpqY6uTvvq4OCEPm\r\nk5um54kjY3ZZ9j2GIZ7fyQLXcAYLMbasM9d88vf5BP98y4qRtT6QmbzEQnZeIXCc\r\nc+7dQ4WHX+ez5ddxmfXmng79An8WOH7BN2HdNAHcmK7FaqW5I31IO0k5xGoVUmP3\r\nf/d3ItyrXuMXMuoMfazVs9hBTtjufAHOTRa+yom36hjBQ3c/zUtmQLcBVE6xFMzU\r\n9nMvDE3dqxLo/CFYCVObbAuSI/N4WA9lDvE652O2C0bclgR4h7sxCyo3F11DgjO+\r\nqx3tAoGARMiiFISHNvgffE9qSArR0xLHpr/26AFtmC4qQNWNs5lAzCCRcvedBESq\r\nUHlfDeRrOdyuBtssUNX2XO1qVqezA8+3mvXOupObYsrQEu56iWCscJiHEmT1NSrk\r\nY/fqoteU/YMWSdBX20MKBF1IWVndrXHu0NYfhuzJD8AZgg0x3IY=\r\n-----END RSA PRIVATE KEY-----",
                    Selector = "test",
                    UseDkim = true,
                };

                //virtualServerConfig.Routes.Add(mxRoute);
                //virtualServerConfig.Routes.Add(nullRoute);
                virtualServerConfig.DkimPolicies.Add(dkimPolicyConfig);
                virtualServerConfig.Users.Add(user);

                _configContext.VirtualServerConfigs.Add(virtualServerConfig);
                _configContext.SaveChanges();
            }
        }
    }
}
