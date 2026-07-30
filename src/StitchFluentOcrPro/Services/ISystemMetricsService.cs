using System;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.Services
{
    public interface ISystemMetricsService : IDisposable
    {
        float GetCpuUsagePercent();
        long GetRamUsageBytes();
        double CalculatePpm(int processedPages, TimeSpan elapsed);
        TimeSpan EstimateRemainingTime(int remainingPages, double ppm);
        ProcessingStats CaptureSnapshot(
            int totalFiles,
            int completedFiles,
            int failedFiles,
            int skippedFiles,
            int inProgressFiles,
            int totalPages,
            int processedPages,
            TimeSpan elapsed,
            int activeWorkers,
            string currentFileName,
            int currentFilePage,
            int currentFileTotalPages);
    }
}
