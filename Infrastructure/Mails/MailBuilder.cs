using System;
using Core.Enums;
using Core.Interfaces.Mails;
using FluentEmail.Core;
using Infrastructure.Mails.Types;

namespace Infrastructure.Mails
{
    public class MailBuilder : IMailBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public MailBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IFluentEmail BuildMail(EmailType type, string parameters)
        {
            var email = GetMailByType(type);
            return email
                .Prepare(parameters, _serviceProvider);
        }

        private BaseMail GetMailByType(EmailType type)
        {
            switch (type)
            {
                case EmailType.WelcomeMail:
                    return new WelcomeMail();
                case EmailType.MessageMail:
                    return new MessageMail();
            }

            throw new Exception("Mail type not found");
        }
    }
}