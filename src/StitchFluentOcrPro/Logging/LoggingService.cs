using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace StitchFluentOcrPro.Logging
{
    public class LoggingService : ILoggingService
    {
        private readonly ConcurrentQueue<LogEntry> _logs = new ConcurrentQueue<LogEntry>();
        private readonly object _fileLock = new object();
        private readonly string _logFilePath;
        private const int MaxMemoryLogCount = 2000;

        public event Action<LogEntry>? OnLogEmitted;

        public IReadOnlyList<LogEntry> RecentLogs => _logs.ToArray();

        public LoggingService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logFolder = Path.Combine(appData, "StitchFluentOcrPro", "Logs");
            Directory.CreateDirectory(logFolder);
            _logFilePath = Path.Combine(logFolder, $"ocr_log_{DateTime.Now:yyyyMMdd}.txt");
        }

        public void LogDebug(string message, string category = "General") => Log(LogLevel.Debug, message, category);
        public void LogInfo(string message, string category = "General") => Log(LogLevel.Info, message, category);
        public void LogWarning(string message, string category = "General") => Log(LogLevel.Warning, message, category);
        public void LogError(string message, Exception? ex = null, string category = "General") => Log(LogLevel.Error, message, category, ex);

        private void Log(LogLevel level, string message, string category, Exception? ex = null)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Category = category,
                Exception = ex
            };

            _logs.Enqueue(entry);
            while (_logs.Count > MaxMemoryLogCount)
            {
                _logs.TryDequeue(out _);
            }

            // Write to disk asynchronously/safely
            WriteToFile(entry);

            // Notify UI subscribers
            OnLogEmitted?.Invoke(entry);
        }

        private void WriteToFile(LogEntry entry)
        {
            lock (_fileLock)
            {
                try
                {
                    string text = $"[{entry.FormattedTimestamp}] [{entry.Level.ToString().ToUpper()}] [{entry.Category}] {entry.Message}";
                    if (entry.Exception != null)
                    {
                        text += $"{Environment.NewLine}Exception: {entry.Exception}";
                    }
                    File.AppendAllText(_logFilePath, text + Environment.NewLine);
                }
                catch
                {
                    // Ignore file write errors to prevent logger crashing system
                }
            }
        }

        public void ClearLogs()
        {
            while (_logs.TryDequeue(out _)) { }
        }

        public void ExportLogsToFile(string filePath)
        {
            var lines = new List<string>();
            foreach (var log in RecentLogs)
            {
                string line = $"[{log.FormattedTimestamp}] [{log.Level}] [{log.Category}] {log.Message}";
                if (log.Exception != null)
                {
                    line += $" | Ex: {log.Exception.Message}";
                }
                lines.Add(line);
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}
