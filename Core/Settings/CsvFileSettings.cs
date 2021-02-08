namespace Core.Settings
{
    public class CsvFileSettings
    {
        public const string Name = "CsvFileSettings";
        
        public string WelcomeMailFilePath { get; set; }
        public string MessageMailFilePath { get; set; }
        public int TakePerAction { get; set; }
    }
}