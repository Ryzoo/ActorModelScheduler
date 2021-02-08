using Core.Enums;

namespace Domain.Messages.Actions
{
    public record ReadMailFromCsvFileMessage : BaseActionMessage
    {
        public EmailType EmailType;
        public int LinesReadBefore;
        
        public ReadMailFromCsvFileMessage(EmailType emailType, int linesReadBefore)
        {
            EmailType = emailType;
            LinesReadBefore = linesReadBefore;
        }
    }
}