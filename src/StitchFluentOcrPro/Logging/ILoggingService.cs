using System;
using System.Collections.Generic;

namespace StitchFluentOcrPro.Logging
{
    public interface ILoggingService
    {
        event Action<LogEntry> OnLogEmitted;
        IReadOnlyList<LogEntry> RecentLogs { get; }
        
        void LogDebug(string message, string category = "General");
        void LogInfo(string message, string category = "General");
        void LogWarning(string message, string category = "General");
        void LogError(string message, Exception? ex = null, string category = "General");
        void ClearLogs();
        void ExportLogsToFile(string filePath);
    }
}
