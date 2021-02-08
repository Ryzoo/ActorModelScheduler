using System;
using Akka.Actor;
using Akka.Event;
using Core.Interfaces.Mails;
using Domain.Messages.Actions;
using Domain.Messages.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Actors
{
    public class MailSenderActor : UntypedActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly IMailBuilder _mailBuilder;

        public MailSenderActor(IServiceProvider sp)
        {
            _mailBuilder = sp.CreateScope().ServiceProvider.GetRequiredService<IMailBuilder>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SendMailMessage mailMessage: SendMail(mailMessage); break;
            }
        }

        private void SendMail(SendMailMessage message)
        {
            try
            {
                _mailBuilder
                    .BuildMail(message.ScheduledMail.EmailType, message.ScheduledMail.Params)
                    .SendAsync();
            }
            catch (Exception e)
            {
                _log.Error("MailSenderActor have problem with send a email message. Exception: {0}", e.Message);
                return;
            }
            
            Sender.Tell(new RespondMailSendMessage(message.RequestId, message.ScheduledMail.EmailType));
        }
    }
}