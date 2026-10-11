namespace LoggingAPI.Models
{
    public class SystemLog
    {
        public int Id { get; set; }

        public string LogLevel { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }
}
