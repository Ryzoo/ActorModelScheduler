using Core.Enums;

namespace Core.MailModels
{
    public class ScheduledMailModel
    {
        public string Params { get; set; }
        public EmailType EmailType { get; set; }
    }
}