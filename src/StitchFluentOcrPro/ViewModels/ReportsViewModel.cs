using System;
using System.Windows.Input;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.Models;
using StitchFluentOcrPro.Services;

namespace StitchFluentOcrPro.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private ReportSummary? _currentReport;

        public ReportSummary? CurrentReport
        {
            get => _currentReport;
            set => SetProperty(ref _currentReport, value);
        }

        public ICommand ExportJsonCommand { get; }
        public ICommand ExportCsvCommand { get; }

        public ReportsViewModel(IReportService reportService)
        {
            _reportService = reportService;

            ExportJsonCommand = new AsyncRelayCommand(ExportJsonAsync, () => CurrentReport != null);
            ExportCsvCommand = new AsyncRelayCommand(ExportCsvAsync, () => CurrentReport != null);
        }

        public void LoadReport(ReportSummary summary)
        {
            CurrentReport = summary;
        }

        private async System.Threading.Tasks.Task ExportJsonAsync()
        {
            if (CurrentReport == null) return;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Report (*.json)|*.json",
                FileName = $"ocr_report_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (dialog.ShowDialog() == true)
            {
                await _reportService.ExportJsonReportAsync(dialog.FileName, CurrentReport);
            }
        }

        private async System.Threading.Tasks.Task ExportCsvAsync()
        {
            if (CurrentReport == null) return;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Summary (*.csv)|*.csv",
                FileName = $"ocr_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                await _reportService.ExportCsvReportAsync(dialog.FileName, CurrentReport);
            }
        }
    }
}
