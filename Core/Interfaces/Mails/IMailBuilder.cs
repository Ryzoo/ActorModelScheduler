using Core.Enums;
using FluentEmail.Core;

namespace Core.Interfaces.Mails
{
    public interface IMailBuilder
    {
        public IFluentEmail BuildMail(EmailType type, string parameters);
    }
}