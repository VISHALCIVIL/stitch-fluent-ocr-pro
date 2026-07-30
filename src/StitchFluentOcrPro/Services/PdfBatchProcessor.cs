using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StitchFluentOcrPro.Configuration;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Models;
using StitchFluentOcrPro.OCR;
using StitchFluentOcrPro.PDF;

namespace StitchFluentOcrPro.Services
{
    public class PdfBatchProcessor : IPdfBatchProcessor
    {
        private readonly IPdfRendererService _pdfRenderer;
        private readonly IOcrEngineService _ocrEngine;
        private readonly ISearchablePdfCreatorService _pdfCreator;
        private readonly ISystemMetricsService _metricsService;
        private readonly IConfigurationService _configService;
        private readonly IReportService _reportService;
        private readonly ILoggingService _logger;

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        public PdfBatchProcessor(
            IPdfRendererService pdfRenderer,
            IOcrEngineService ocrEngine,
            ISearchablePdfCreatorService pdfCreator,
            ISystemMetricsService metricsService,
            IConfigurationService configService,
            IReportService reportService,
            ILoggingService logger)
        {
            _pdfRenderer = pdfRenderer;
            _ocrEngine = ocrEngine;
            _pdfCreator = pdfCreator;
            _metricsService = metricsService;
            _configService = configService;
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<ReportSummary> ProcessBatchAsync(
            string inputFolder,
            string outputFolder,
            List<PdfJob> jobs,
            CancellationToken cancellationToken,
            PauseToken pauseToken,
            Action<ProcessingStats>? progressCallback = null)
        {
            IsRunning = true;
            IsPaused = false;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInfo($"Starting high-performance batch processing of {jobs.Count} PDF jobs.", "Engine");

            int activeWorkers = 0;
            int totalProcessedPages = 0;
            int grandTotalPages = 0;

            // Pre-calculate page counts for queue tracking
            var pendingJobs = jobs.Where(j => j.Status == JobStatus.Queued).ToList();

            int maxConcurrency = Math.Max(1, _configService.Settings.MaxDegreeOfParallelism);
            int dpi = Math.Clamp(_configService.Settings.RenderDpi, 100, 300);
            string langTag = _configService.Settings.SelectedLanguageTag;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxConcurrency,
                CancellationToken = cancellationToken
            };

            // Start timer for UI metrics heartbeat updates
            using var ctsMetrics = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                while (!ctsMetrics.Token.IsCancellationRequested && IsRunning)
                {
                    try
                    {
                        await Task.Delay(500, ctsMetrics.Token);
                        if (progressCallback != null)
                        {
                            int completed = jobs.Count(j => j.Status == JobStatus.Completed);
                            int failed = jobs.Count(j => j.Status == JobStatus.Failed);
                            int skipped = jobs.Count(j => j.Status == JobStatus.Skipped);
                            int inProg = jobs.Count(j => j.Status == JobStatus.Processing);

                            string currentFile = jobs.FirstOrDefault(j => j.Status == JobStatus.Processing)?.FileName ?? string.Empty;

                            var snapshot = _metricsService.CaptureSnapshot(
                                totalFiles: jobs.Count,
                                completedFiles: completed,
                                failedFiles: failed,
                                skippedFiles: skipped,
                                inProgressFiles: inProg,
                                totalPages: grandTotalPages > 0 ? grandTotalPages : jobs.Sum(j => j.TotalPages),
                                processedPages: totalProcessedPages,
                                elapsed: stopwatch.Elapsed,
                                activeWorkers: activeWorkers,
                                currentFileName: currentFile,
                                currentFilePage: 0,
                                currentFileTotalPages: 0);

                            progressCallback(snapshot);
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch { }
                }
            }, ctsMetrics.Token);

            try
            {
                await Parallel.ForEachAsync(pendingJobs, parallelOptions, async (job, ct) =>
                {
                    Interlocked.Increment(ref activeWorkers);
                    job.Status = JobStatus.Processing;
                    job.StartTime = DateTime.Now;

                    _logger.LogInfo($"[Worker] Starting parallel OCR on file: {job.FileName}", "Engine");

                    try
                    {
                        // 1. Get Page Count & Dimensions
                        int pageCount = await _pdfRenderer.GetPageCountAsync(job.InputPath);
                        job.TotalPages = pageCount;
                        Interlocked.Add(ref grandTotalPages, pageCount);

                        var pageResultsArray = new OcrPageResult[pageCount];
                        var pageIndices = Enumerable.Range(0, pageCount).ToList();

                        // 2. High-speed Parallel Page Processing across CPU cores
                        var pageParallelOptions = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = maxConcurrency,
                            CancellationToken = ct
                        };

                        await Parallel.ForEachAsync(pageIndices, pageParallelOptions, async (pIndex, pageCt) =>
                        {
                            pageCt.ThrowIfCancellationRequested();
                            await pauseToken.WaitWhilePausedAsync(pageCt);

                            var (wPts, hPts) = await _pdfRenderer.GetPageDimensionsAsync(job.InputPath, pIndex);
                            using var imgStream = await _pdfRenderer.RenderPageToImageStreamAsync(job.InputPath, pIndex, dpi);

                            var pageOcr = await _ocrEngine.ProcessImageStreamAsync(
                                imgStream, 
                                pIndex, 
                                wPts, 
                                hPts, 
                                langTag);

                            pageResultsArray[pIndex] = pageOcr;

                            job.ProcessedPages++;
                            Interlocked.Increment(ref totalProcessedPages);
                        });

                        var pageResults = pageResultsArray.Where(p => p != null).OrderBy(p => p.PageIndex).ToList();

                        // 3. Construct Searchable PDF Output
                        await _pdfCreator.CreateSearchablePdfAsync(job.OutputPath, pageResults);

                        job.EndTime = DateTime.Now;
                        job.Duration = job.EndTime.Value - job.StartTime.Value;
                        job.Status = JobStatus.Completed;

                        _logger.LogInfo($"[Worker] Successfully completed: {job.FileName} ({pageCount} pages, {job.Duration.TotalSeconds:F1}s)", "Engine");

                        // Cleanups
                        pageResults.Clear();
                        GC.Collect(0, GCCollectionMode.Optimized, false);
                    }
                    catch (OperationCanceledException)
                    {
                        job.Status = JobStatus.Paused;
                        _logger.LogWarning($"Processing canceled for job: {job.FileName}", "Engine");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        job.Status = JobStatus.Failed;
                        job.ErrorMessage = ex.Message;
                        job.EndTime = DateTime.Now;
                        if (job.StartTime.HasValue)
                        {
                            job.Duration = job.EndTime.Value - job.StartTime.Value;
                        }
                        _logger.LogError($"[Worker] Failed processing file '{job.FileName}'.", ex, "Engine");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref activeWorkers);
                    }
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Batch processing was stopped by user request.", "Engine");
            }
            finally
            {
                ctsMetrics.Cancel();
                stopwatch.Stop();
                IsRunning = false;
                IsPaused = false;
            }

            return _reportService.GenerateSummary(inputFolder, outputFolder, jobs, stopwatch.Elapsed);
        }
    }
}
