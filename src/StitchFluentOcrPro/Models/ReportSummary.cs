using System;
using System.Collections.Generic;

namespace StitchFluentOcrPro.Models
{
    /// <summary>
    /// Summary report generated after batch OCR execution completes.
    /// </summary>
    public sealed class ReportSummary
    {
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public string InputFolder { get; set; } = string.Empty;
        public string OutputFolder { get; set; } = string.Empty;
        
        public int TotalFiles { get; set; }
        public int SuccessfulFiles { get; set; }
        public int FailedFiles { get; set; }
        public int SkippedFiles { get; set; }

        public int TotalPagesProcessed { get; set; }
        public TimeSpan TotalExecutionTime { get; set; }
        public double AveragePagesPerMinute { get; set; }
        public long TotalBytesProcessed { get; set; }

        public List<PdfJob> JobDetails { get; set; } = new List<PdfJob>();
    }
}
