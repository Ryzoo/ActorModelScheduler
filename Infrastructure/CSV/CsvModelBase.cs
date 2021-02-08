using Core.Enums;
using Core.MailModels;
using Newtonsoft.Json;

namespace Infrastructure.CSV
{
    public abstract class CsvModelBase<T>
    {
        public ScheduledMailModel ToDomainModel(EmailType type) => new()
        {
            Params = JsonConvert.SerializeObject(this),
            EmailType = type
        };
    }
}