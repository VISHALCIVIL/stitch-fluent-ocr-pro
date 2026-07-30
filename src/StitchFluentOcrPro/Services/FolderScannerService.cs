using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Models;
using StitchFluentOcrPro.Utilities;

namespace StitchFluentOcrPro.Services
{
    public class FolderScannerService : IFolderScannerService
    {
        private readonly ILoggingService _logger;

        public FolderScannerService(ILoggingService logger)
        {
            _logger = logger;
        }

        public Task<List<PdfJob>> ScanFolderAsync(string inputFolder, string outputFolder, bool skipExisting = true)
        {
            return Task.Run(() =>
            {
                var jobs = new List<PdfJob>();

                if (string.IsNullOrWhiteSpace(inputFolder) || !Directory.Exists(inputFolder))
                {
                    _logger.LogWarning($"Input directory does not exist or is invalid: {inputFolder}");
                    return jobs;
                }

                try
                {
                    var files = Directory.GetFiles(inputFolder, "*.pdf", SearchOption.AllDirectories);
                    _logger.LogInfo($"Discovered {files.Length} PDF file(s) in input directory.", "Scanner");

                    foreach (var filePath in files)
                    {
                        string outputPath = FileHelper.ResolveOutputPath(inputFolder, outputFolder, filePath);
                        long size = FileHelper.GetFileSizeSafely(filePath);
                        string relativeDir = Path.GetDirectoryName(Path.GetRelativePath(inputFolder, filePath)) ?? string.Empty;

                        bool alreadyExists = skipExisting && File.Exists(outputPath);

                        var job = new PdfJob
                        {
                            InputPath = filePath,
                            OutputPath = outputPath,
                            FileName = Path.GetFileName(filePath),
                            RelativeDirectory = relativeDir,
                            FileSizeBytes = size,
                            Status = alreadyExists ? JobStatus.Skipped : JobStatus.Queued,
                            ProcessedPages = 0,
                            TotalPages = 0
                        };

                        if (alreadyExists)
                        {
                            _logger.LogInfo($"Skipping pre-existing output PDF: {job.FileName}", "Scanner");
                        }

                        jobs.Add(job);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error scanning directory '{inputFolder}'.", ex, "Scanner");
                }

                return jobs;
            });
        }
    }
}
