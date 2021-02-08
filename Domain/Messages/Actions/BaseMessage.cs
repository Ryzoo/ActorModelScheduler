using System;

namespace Domain.Messages.Actions
{
    public abstract record BaseActionMessage
    {
        public Guid RequestId = Guid.NewGuid();
    }
}