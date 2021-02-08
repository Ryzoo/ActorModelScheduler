using System;
using System.IO;
using Core.Interfaces.Mails;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mails.Types
{
    public abstract class BaseMail : IEmail
    {
        protected string To = "to@mail.com";
        protected string Subject;
        protected string Template;

        public IFluentEmail Prepare(string mailData, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var currentDirectory = Directory.GetCurrentDirectory();
            var file = Path.Combine(currentDirectory, "..", "Infrastructure", "Mails", "Template",$"{Template}.cshtml");
            
            return scope.ServiceProvider.GetRequiredService<IFluentEmail>()
                .To(To)
                .Subject(Subject)
                .UsingTemplateFromFile(file, PrepareParams(mailData));
        }

        protected abstract IEmailTemplateParams PrepareParams(string param);
    }
}