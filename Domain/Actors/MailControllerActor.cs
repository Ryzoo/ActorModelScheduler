using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Core.Enums;
using Core.Settings;
using Domain.Messages.Actions;
using Domain.Messages.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceProvider = Akka.DependencyInjection.ServiceProvider;

namespace Domain.Actors
{
    public class MailControllerActor : UntypedActor
    {
        private readonly int _maxMailsPerMinute;
        private int _mailsWithoutStatus;
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly IActorRef _mailFileReaderActor;
        private readonly IActorRef _mailSenderActor;
        private readonly Dictionary<DateTime, int> _sentMailsPerMinute;
        private readonly Dictionary<EmailType, int> _sentMailsCount;

        public MailControllerActor(IServiceProvider sp)
        {
            var mailFileReaderProps = ServiceProvider.For(Context.System).Props<MailFileReaderActor>();
            var mailSenderProps = ServiceProvider.For(Context.System).Props<MailSenderActor>();

            _sentMailsPerMinute = new Dictionary<DateTime, int>();
            _sentMailsCount = new Dictionary<EmailType, int>();
            _mailFileReaderActor = Context.ActorOf(mailFileReaderProps, "MailFileReaderActor");
            _mailSenderActor = Context.ActorOf(mailSenderProps, "MailSenderActor");
            _maxMailsPerMinute = sp.CreateScope().ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value
                .MaxPerMinute;
            _mailsWithoutStatus = 0;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StandUpMessage when GetMailsInCurrentMinute() < _maxMailsPerMinute:
                    CheckCsvFile();
                    break;
                case RespondMailFromCsvFileMessage response:
                    TrySendMails(response);
                    break;
                case RespondMailSendMessage response:
                    UpdateMailSentCount(response);
                    break;
            }
        }

        private void CheckCsvFile()
        {
            _mailFileReaderActor.Tell(new ReadMailFromCsvFileMessage(EmailType.WelcomeMail,
                GetLastLine(EmailType.WelcomeMail)));
            _mailFileReaderActor.Tell(new ReadMailFromCsvFileMessage(EmailType.MessageMail,
                GetLastLine(EmailType.MessageMail)));
        }

        private void UpdateMailSentCount(RespondMailSendMessage response)
        {
            var nowTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                DateTime.Now.Minute, 0, 0);

            if (_sentMailsPerMinute.TryGetValue(nowTime, out _)) _sentMailsPerMinute[nowTime]++;
            else _sentMailsPerMinute.Add(nowTime, 1);

            if (_sentMailsCount.TryGetValue(response.EmailType, out _)) _sentMailsCount[response.EmailType]++;
            else _sentMailsCount.Add(response.EmailType, 1);

            _log.Info("{0} was sent {1} times.", response.EmailType, GetLastLine(response.EmailType));
            _log.Warning("{0} of {1} mails was sent in current minute: {2}", _sentMailsPerMinute[nowTime], _maxMailsPerMinute, nowTime.Minute);
            _mailsWithoutStatus--;
        }

        private void TrySendMails(RespondMailFromCsvFileMessage response)
        {
            var actualMailsCount = GetMailsInCurrentMinute() + _mailsWithoutStatus;
            
            foreach (var scheduledMail in response.Mails.Take(_maxMailsPerMinute - actualMailsCount))
            {
                _mailSenderActor.Tell(new SendMailMessage(scheduledMail));
                _mailsWithoutStatus++;
            }
        }

        private int GetLastLine(EmailType emailType) =>
            _sentMailsCount.TryGetValue(emailType, out int value) ? value : 0;

        private int GetMailsInCurrentMinute()
        {
            var nowTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                DateTime.Now.Minute, 0, 0);

            return _sentMailsPerMinute.TryGetValue(nowTime, out _) ? _sentMailsPerMinute[nowTime] : 0;
        }
    }
}