using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using StitchFluentOcrPro.Configuration;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Models;
using StitchFluentOcrPro.Services;

namespace StitchFluentOcrPro.ViewModels
{
    public class ProcessPdfViewModel : ViewModelBase
    {
        private readonly IFolderScannerService _scannerService;
        private readonly IPdfBatchProcessor _batchProcessor;
        private readonly IConfigurationService _configService;
        private readonly ILoggingService _logger;

        private string _inputFolder = string.Empty;
        private string _outputFolder = string.Empty;
        private bool _isScanning;
        private bool _isProcessing;
        private bool _isPaused;
        private double _overallProgress;
        private ProcessingStats _currentStats = new ProcessingStats();

        private CancellationTokenSource? _cts;
        private PauseTokenSource? _pts;

        public ObservableCollection<PdfJob> Jobs { get; } = new ObservableCollection<PdfJob>();

        public string InputFolder
        {
            get => _inputFolder;
            set
            {
                if (SetProperty(ref _inputFolder, value))
                {
                    _configService.Settings.InputFolder = value;
                    _configService.Save();
                }
            }
        }

        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                if (SetProperty(ref _outputFolder, value))
                {
                    _configService.Settings.OutputFolder = value;
                    _configService.Save();
                }
            }
        }

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        public double OverallProgress
        {
            get => _overallProgress;
            set => SetProperty(ref _overallProgress, value);
        }

        public ProcessingStats CurrentStats
        {
            get => _currentStats;
            set => SetProperty(ref _currentStats, value);
        }

        public ICommand ScanFolderCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand StopCommand { get; }

        public event Action<ReportSummary>? OnBatchCompleted;

        public ProcessPdfViewModel(
            IFolderScannerService scannerService,
            IPdfBatchProcessor batchProcessor,
            IConfigurationService configService,
            ILoggingService logger)
        {
            _scannerService = scannerService;
            _batchProcessor = batchProcessor;
            _configService = configService;
            _logger = logger;

            InputFolder = _configService.Settings.InputFolder;
            OutputFolder = _configService.Settings.OutputFolder;

            ScanFolderCommand = new AsyncRelayCommand(ScanFolderAsync, () => !IsProcessing && !IsScanning);
            StartCommand = new AsyncRelayCommand(StartAsync, () => !IsProcessing && Jobs.Count > 0);
            PauseCommand = new RelayCommand(Pause, () => IsProcessing && !IsPaused);
            ResumeCommand = new RelayCommand(Resume, () => IsProcessing && IsPaused);
            StopCommand = new RelayCommand(Stop, () => IsProcessing);
        }

        public async Task ScanFolderAsync()
        {
            if (string.IsNullOrWhiteSpace(InputFolder) || !Directory.Exists(InputFolder))
            {
                _logger.LogWarning("Please select a valid input directory before scanning.");
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputFolder))
            {
                // Auto set default output folder sibling to input folder
                OutputFolder = Path.Combine(Directory.GetParent(InputFolder)?.FullName ?? InputFolder, "OCR_Output");
            }

            try
            {
                IsScanning = true;
                Jobs.Clear();

                var discovered = await _scannerService.ScanFolderAsync(
                    InputFolder, 
                    OutputFolder, 
                    _configService.Settings.SkipExistingFiles);

                foreach (var job in discovered)
                {
                    Jobs.Add(job);
                }

                _logger.LogInfo($"Scanning complete. {Jobs.Count} PDF jobs ready for OCR processing.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed during directory scan.", ex);
            }
            finally
            {
                IsScanning = false;
            }
        }

        public async Task StartAsync()
        {
            if (Jobs.Count == 0)
            {
                await ScanFolderAsync();
            }

            if (Jobs.Count == 0)
            {
                _logger.LogWarning("No PDF files available in queue to process.");
                return;
            }

            IsProcessing = true;
            IsPaused = false;
            _cts = new CancellationTokenSource();
            _pts = new PauseTokenSource();

            try
            {
                ReportSummary report = await _batchProcessor.ProcessBatchAsync(
                    InputFolder,
                    OutputFolder,
                    new System.Collections.Generic.List<PdfJob>(Jobs),
                    _cts.Token,
                    _pts.Token,
                    stats =>
                    {
                        CurrentStats = stats;
                        if (stats.TotalFiles > 0)
                        {
                            OverallProgress = Math.Min(100.0, ((double)(stats.CompletedFiles + stats.FailedFiles + stats.SkippedFiles) / stats.TotalFiles) * 100.0);
                        }
                    });

                _logger.LogInfo($"Batch execution completed cleanly in {report.TotalExecutionTime.TotalSeconds:F1}s.");
                OnBatchCompleted?.Invoke(report);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Batch processing stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Batch processing encountered a fatal error.", ex);
            }
            finally
            {
                IsProcessing = false;
                IsPaused = false;
            }
        }

        public void Pause()
        {
            _pts?.Pause();
            IsPaused = true;
            _logger.LogInfo("Batch execution paused by user.");
        }

        public void Resume()
        {
            _pts?.Resume();
            IsPaused = false;
            _logger.LogInfo("Batch execution resumed.");
        }

        public void Stop()
        {
            _cts?.Cancel();
            _pts?.Resume(); // Unblock paused workers so they can observe cancellation token
            IsProcessing = false;
            IsPaused = false;
            _logger.LogWarning("Batch execution stop request sent.");
        }
    }
}
