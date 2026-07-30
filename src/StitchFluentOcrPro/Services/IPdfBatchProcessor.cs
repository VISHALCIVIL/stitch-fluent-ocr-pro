using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.Services
{
    public interface IPdfBatchProcessor
    {
        bool IsRunning { get; }
        bool IsPaused { get; }
        
        Task<ReportSummary> ProcessBatchAsync(
            string inputFolder,
            string outputFolder,
            List<PdfJob> jobs,
            CancellationToken cancellationToken,
            PauseToken pauseToken,
            Action<ProcessingStats>? progressCallback = null);
    }
}
