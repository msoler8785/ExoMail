namespace ExoMail.Smtp.Common
{
    public class QueueOptions
    {
        public string QueuePath { get; set; }
        public string ArchivePath { get; set; }
        public bool DefaultRouteEnabled { get; set; }

        public QueueOptions()
        {
        }
    }
}