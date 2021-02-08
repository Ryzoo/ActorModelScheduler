using CsvHelper.Configuration.Attributes;

namespace Infrastructure.CSV
{
    public class WelcomeMailDataCsvModel : CsvModelBase<WelcomeMailDataCsvModel>
    {
        [Name("name")]
        public string UserName { get; set; }
        [Name("email")]
        public string UserEmail { get; set; }
    }
}