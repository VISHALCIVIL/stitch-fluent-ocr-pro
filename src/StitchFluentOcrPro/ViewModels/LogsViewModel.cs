using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.Logging;

namespace StitchFluentOcrPro.ViewModels
{
    public class LogsViewModel : ViewModelBase
    {
        private readonly ILoggingService _logger;
        private readonly object _lock = new object();

        private string _selectedLogLevel = "All";
        private string _logSearchQuery = string.Empty;
        private bool _autoScroll = true;

        public ObservableCollection<LogEntry> Logs { get; } = new ObservableCollection<LogEntry>();

        public string SelectedLogLevel
        {
            get => _selectedLogLevel;
            set
            {
                if (SetProperty(ref _selectedLogLevel, value))
                {
                    RefreshView();
                }
            }
        }

        public string LogSearchQuery
        {
            get => _logSearchQuery;
            set
            {
                if (SetProperty(ref _logSearchQuery, value))
                {
                    RefreshView();
                }
            }
        }

        public bool AutoScroll
        {
            get => _autoScroll;
            set => SetProperty(ref _autoScroll, value);
        }

        public ICommand ClearLogsCommand { get; }
        public ICommand ExportLogsCommand { get; }

        public LogsViewModel(ILoggingService logger)
        {
            _logger = logger;
            BindingOperations.EnableCollectionSynchronization(Logs, _lock);

            ClearLogsCommand = new RelayCommand(ClearLogs);
            ExportLogsCommand = new RelayCommand(ExportLogs);

            _logger.OnLogEmitted += OnLogEmitted;

            // Load existing logs
            foreach (var log in _logger.RecentLogs)
            {
                Logs.Add(log);
            }
        }

        private void OnLogEmitted(LogEntry entry)
        {
            lock (_lock)
            {
                if (MatchesFilter(entry))
                {
                    Logs.Add(entry);
                }
            }
        }

        private bool MatchesFilter(LogEntry entry)
        {
            if (SelectedLogLevel != "All" && Enum.TryParse<LogLevel>(SelectedLogLevel, out var level))
            {
                if (entry.Level != level) return false;
            }

            if (!string.IsNullOrWhiteSpace(LogSearchQuery))
            {
                return entry.Message.Contains(LogSearchQuery, StringComparison.OrdinalIgnoreCase) ||
                       entry.Category.Contains(LogSearchQuery, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }

        private void RefreshView()
        {
            lock (_lock)
            {
                Logs.Clear();
                foreach (var log in _logger.RecentLogs.Where(MatchesFilter))
                {
                    Logs.Add(log);
                }
            }
        }

        private void ClearLogs()
        {
            _logger.ClearLogs();
            lock (_lock)
            {
                Logs.Clear();
            }
        }

        private void ExportLogs()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = $"ocr_export_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                _logger.ExportLogsToFile(dialog.FileName);
            }
        }
    }
}
