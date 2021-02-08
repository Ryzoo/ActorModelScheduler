namespace Core.Settings
{
    public class EmailSettings
    {
        public const string Name = "EmailSettings";
        
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int MaxPerMinute { get; set; }
    }
}