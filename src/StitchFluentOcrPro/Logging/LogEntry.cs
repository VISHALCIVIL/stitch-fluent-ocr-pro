using System;

namespace StitchFluentOcrPro.Logging
{
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public LogLevel Level { get; set; } = LogLevel.Info;
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public Exception? Exception { get; set; }

        public string FormattedTimestamp => Timestamp.ToString("HH:mm:ss.fff");
    }
}
