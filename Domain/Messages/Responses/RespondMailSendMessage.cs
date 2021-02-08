using System;
using Core.Enums;

namespace Domain.Messages.Responses
{
    public record RespondMailSendMessage : BaseResponseMessage
    {
        public EmailType EmailType;
        
        public RespondMailSendMessage(Guid requestId, EmailType emailType): base(requestId)
        {
            EmailType = emailType;
        }
    }
}