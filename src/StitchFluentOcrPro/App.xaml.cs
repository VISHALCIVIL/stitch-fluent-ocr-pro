using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StitchFluentOcrPro.Configuration;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.OCR;
using StitchFluentOcrPro.PDF;
using StitchFluentOcrPro.Services;
using StitchFluentOcrPro.ViewModels;

namespace StitchFluentOcrPro
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Core Infrastructure & Configuration
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<ILoggingService, LoggingService>();

            // OCR & PDF Engines
            services.AddSingleton<IOcrEngineService, WindowsOcrEngineService>();
            services.AddSingleton<IPdfRendererService, WindowsPdfRendererService>();
            services.AddSingleton<ISearchablePdfCreatorService, SearchablePdfCreatorService>();

            // Application Services
            services.AddSingleton<IFolderScannerService, FolderScannerService>();
            services.AddSingleton<ISystemMetricsService, SystemMetricsService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IPdfBatchProcessor, PdfBatchProcessor>();

            // ViewModels
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<ProcessPdfViewModel>();
            services.AddSingleton<QueueViewModel>();
            services.AddSingleton<LogsViewModel>();
            services.AddSingleton<ReportsViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AboutViewModel>();
            services.AddSingleton<MainViewModel>();

            // UI Main Window
            services.AddSingleton<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}
