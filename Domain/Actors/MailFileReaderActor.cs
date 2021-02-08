using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Core.Enums;
using Core.Interfaces;
using Core.MailModels;
using Core.Settings;
using Domain.Messages.Actions;
using Domain.Messages.Responses;
using Infrastructure.CSV;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Domain.Actors
{
    public class MailFileReaderActor : UntypedActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly ICsvParser _csvParser;
        private readonly IOptions<CsvFileSettings> _settings;

        public MailFileReaderActor(IServiceProvider sp)
        {
            var scope = sp.CreateScope();
            _csvParser = scope.ServiceProvider.GetRequiredService<ICsvParser>();
            _settings = scope.ServiceProvider.GetRequiredService<IOptions<CsvFileSettings>>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case ReadMailFromCsvFileMessage read: ReadEmailsFromFile(read); break;
            }
        }

        private void ReadEmailsFromFile(ReadMailFromCsvFileMessage message)
        {
            List<ScheduledMailModel> mails = new List<ScheduledMailModel>();
            
            try
            {
                switch (message.EmailType)
                {
                    case EmailType.WelcomeMail:
                        mails = ReadMails<WelcomeMailDataCsvModel>(_settings.Value.WelcomeMailFilePath, message.EmailType, message.LinesReadBefore);
                        break;
                    case EmailType.MessageMail:
                        mails = ReadMails<MessageMailDataCsvModel>(_settings.Value.MessageMailFilePath, message.EmailType, message.LinesReadBefore);
                        break;
                }
            }
            catch (Exception)
            {
                _log.Error("MailFileReaderActor have problem with read emails");
                return;
            }

            if (mails.Any()) Sender.Tell(new RespondMailFromCsvFileMessage(message.RequestId, mails));
        }
        
        private List<ScheduledMailModel> ReadMails<T>(string path, EmailType type, int linesBefore)
        where T : CsvModelBase<T>
        {
            return _csvParser
                .ReadCsvFile<T>(path, linesBefore, _settings.Value.TakePerAction)
                .Select(x => x.ToDomainModel(type))
                .ToList();
        }
    }
}