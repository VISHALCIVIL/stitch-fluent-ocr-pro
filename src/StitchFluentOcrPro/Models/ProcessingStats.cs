using System;

namespace StitchFluentOcrPro.Models
{
    /// <summary>
    /// Holds live telemetry, operational statistics, and system resource metrics for the processing engine.
    /// </summary>
    public sealed class ProcessingStats
    {
        public int TotalFiles { get; set; }
        public int CompletedFiles { get; set; }
        public int FailedFiles { get; set; }
        public int SkippedFiles { get; set; }
        public int InProgressFiles { get; set; }

        public int TotalPages { get; set; }
        public int ProcessedPages { get; set; }
        public double PagesPerMinute { get; set; }

        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan EstimatedRemainingTime { get; set; }

        public float CpuUsagePercent { get; set; }
        public long RamUsageBytes { get; set; }
        public int ActiveWorkers { get; set; }

        public string CurrentFileName { get; set; } = string.Empty;
        public int CurrentFilePage { get; set; }
        public int CurrentFileTotalPages { get; set; }
    }
}
