using ExoMail.Smtp.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    internal class SmtpUnknownCommand : SmtpCommandBase
    {
        public override bool ArgumentsValid
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => { return SmtpResponse.CommandUnrecognized; } );
        }
    }
}