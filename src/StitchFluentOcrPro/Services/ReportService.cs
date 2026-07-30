using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.Services
{
    public class ReportService : IReportService
    {
        private readonly ILoggingService _logger;

        public ReportService(ILoggingService logger)
        {
            _logger = logger;
        }

        public ReportSummary GenerateSummary(
            string inputFolder,
            string outputFolder,
            IReadOnlyList<PdfJob> jobs,
            TimeSpan totalExecutionTime)
        {
            int totalFiles = jobs.Count;
            int successful = jobs.Count(j => j.Status == JobStatus.Completed);
            int failed = jobs.Count(j => j.Status == JobStatus.Failed);
            int skipped = jobs.Count(j => j.Status == JobStatus.Skipped);

            int totalPages = jobs.Sum(j => j.ProcessedPages);
            long totalBytes = jobs.Sum(j => j.FileSizeBytes);

            double ppm = totalExecutionTime.TotalMinutes > 0
                ? totalPages / totalExecutionTime.TotalMinutes
                : 0.0;

            return new ReportSummary
            {
                GeneratedAt = DateTime.Now,
                InputFolder = inputFolder,
                OutputFolder = outputFolder,
                TotalFiles = totalFiles,
                SuccessfulFiles = successful,
                FailedFiles = failed,
                SkippedFiles = skipped,
                TotalPagesProcessed = totalPages,
                TotalExecutionTime = totalExecutionTime,
                AveragePagesPerMinute = ppm,
                TotalBytesProcessed = totalBytes,
                JobDetails = jobs.ToList()
            };
        }

        public async Task ExportJsonReportAsync(string filePath, ReportSummary summary)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(summary, options);
                await File.WriteAllTextAsync(filePath, json);
                _logger.LogInfo($"Exported JSON report to: {filePath}", "Report");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to export JSON report to {filePath}.", ex, "Report");
            }
        }

        public async Task ExportCsvReportAsync(string filePath, ReportSummary summary)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("FileName,RelativeDirectory,Status,ProcessedPages,TotalPages,FileSizeBytes,DurationSeconds,ErrorMessage");

                foreach (var job in summary.JobDetails)
                {
                    string safeErr = job.ErrorMessage.Replace("\"", "\"\"");
                    sb.AppendLine($"\"{job.FileName}\",\"{job.RelativeDirectory}\",\"{job.Status}\",{job.ProcessedPages},{job.TotalPages},{job.FileSizeBytes},{job.Duration.TotalSeconds:F2},\"{safeErr}\"");
                }

                await File.WriteAllTextAsync(filePath, sb.ToString());
                _logger.LogInfo($"Exported CSV report to: {filePath}", "Report");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to export CSV report to {filePath}.", ex, "Report");
            }
        }
    }
}
