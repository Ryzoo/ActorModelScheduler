using Core.MailModels;

namespace Domain.Messages.Actions
{
    public record SendMailMessage : BaseActionMessage
    {
        public ScheduledMailModel ScheduledMail;
        
        public SendMailMessage(ScheduledMailModel scheduledMail)
        {
            ScheduledMail = scheduledMail;
        }
    }
}