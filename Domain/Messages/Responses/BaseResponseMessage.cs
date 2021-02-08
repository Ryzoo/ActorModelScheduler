using System;

namespace Domain.Messages.Responses
{
    public abstract record BaseResponseMessage
    {
        public Guid RequestId;

        protected BaseResponseMessage(Guid guid)
        {
            RequestId = guid;
        }
    }
}