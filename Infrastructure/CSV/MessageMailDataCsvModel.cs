using CsvHelper.Configuration.Attributes;

namespace Infrastructure.CSV
{
    public class MessageMailDataCsvModel : CsvModelBase<MessageMailDataCsvModel>
    {
        [Name("name")]
        public string UserName { get; set; }
        [Name("email")]
        public string UserEmail { get; set; }
        [Name("message")]
        public string Message { get; set; }
    }
}