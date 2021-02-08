using System;
using System.Collections.Generic;
using Core.MailModels;

namespace Domain.Messages.Responses
{
    public record RespondMailFromCsvFileMessage : BaseResponseMessage
    {
        public IReadOnlyCollection<ScheduledMailModel> Mails;
        
        public RespondMailFromCsvFileMessage(Guid requestId, IReadOnlyCollection<ScheduledMailModel> mails): base(requestId)
        {
            RequestId = requestId;
            Mails = mails;
        }
    }
}