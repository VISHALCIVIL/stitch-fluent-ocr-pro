using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StitchFluentOcrPro.Models
{
    /// <summary>
    /// Represents an individual PDF file job queued for OCR processing.
    /// Implements INotifyPropertyChanged for real-time WPF DataGrid updates.
    /// </summary>
    public class PdfJob : INotifyPropertyChanged
    {
        private JobStatus _status = JobStatus.Queued;
        private int _processedPages;
        private int _totalPages;
        private string _errorMessage = string.Empty;
        private TimeSpan _duration;
        private double _progressPercentage;

        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string RelativeDirectory { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }

        public JobStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProcessedPages
        {
            get => _processedPages;
            set
            {
                if (_processedPages != value)
                {
                    _processedPages = value;
                    OnPropertyChanged();
                    UpdateProgress();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    OnPropertyChanged();
                    UpdateProgress();
                }
            }
        }

        public double ProgressPercentage
        {
            get => _progressPercentage;
            private set
            {
                if (Math.Abs(_progressPercentage - value) > 0.01)
                {
                    _progressPercentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        private void UpdateProgress()
        {
            if (TotalPages > 0)
            {
                ProgressPercentage = Math.Min(100.0, ((double)ProcessedPages / TotalPages) * 100.0);
            }
            else
            {
                ProgressPercentage = 0.0;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
