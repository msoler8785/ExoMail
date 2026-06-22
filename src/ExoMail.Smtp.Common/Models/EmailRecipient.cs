using ExoMail.Smtp.Common.Enums;
using System;

namespace ExoMail.Smtp.Common.Models
{
    public class EmailRecipient
    {
        //public int EmailRecipientId { get; set; }
        public MailAddress MailAddress { get; set; }
        public string Domain => MailAddress?.Domain;
        public DateTime SentDate { get; set; }
        public DateTime NextAttempt { get; set; }
        public string LastResponse { get; set; }
        public Disposition Disposition { get; set; }
        public int DeliveryAttempts { get; set; }

        public bool DeliveryComplete => Disposition.HasDisposition(Disposition.Delivered | Disposition.PermFail);
        public bool DeliveryRetryable => Disposition.HasDisposition(Disposition.Retryable);

        public EmailRecipient() { }

        public EmailRecipient(string emailAddress)
            : this(MailAddress.Parse(emailAddress)) { }
     

        public EmailRecipient(MailAddress mailAddress)
        {
            MailAddress = mailAddress;
            NextAttempt = DateTime.MinValue;
            DeliveryAttempts = 0;
        }

        public void MarkAsDelivered(string lastResponse)
        {
            DeliveryAttempts++;
            Disposition = Disposition.Delivered;
            SentDate = DateTime.Now;
            LastResponse = lastResponse;
        }

        public void TempFail(string lastResponse, TimeSpan deferral)
        {
            DeliveryAttempts++;
            Disposition = Disposition.TempFail;
            NextAttempt = DateTime.Now.Add(deferral);
            LastResponse = lastResponse;
        }

        public void PermFail(string lastResponse)
        {
            DeliveryAttempts++;
            Disposition = Disposition.PermFail;
            LastResponse = lastResponse;
        }
    }
}