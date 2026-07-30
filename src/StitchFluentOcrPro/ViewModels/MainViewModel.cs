using System.Windows.Input;
using StitchFluentOcrPro.Infrastructure;

namespace StitchFluentOcrPro.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;
        private string _activePageName = "Dashboard";

        public DashboardViewModel DashboardVM { get; }
        public ProcessPdfViewModel ProcessPdfVM { get; }
        public QueueViewModel QueueVM { get; }
        public LogsViewModel LogsVM { get; }
        public ReportsViewModel ReportsVM { get; }
        public SettingsViewModel SettingsVM { get; }
        public AboutViewModel AboutVM { get; }

        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string ActivePageName
        {
            get => _activePageName;
            set => SetProperty(ref _activePageName, value);
        }

        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateProcessPdfCommand { get; }
        public ICommand NavigateQueueCommand { get; }
        public ICommand NavigateLogsCommand { get; }
        public ICommand NavigateReportsCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateAboutCommand { get; }

        public MainViewModel(
            DashboardViewModel dashboardVM,
            ProcessPdfViewModel processPdfVM,
            QueueViewModel queueVM,
            LogsViewModel logsVM,
            ReportsViewModel reportsVM,
            SettingsViewModel settingsVM,
            AboutViewModel aboutVM)
        {
            DashboardVM = dashboardVM;
            ProcessPdfVM = processPdfVM;
            QueueVM = queueVM;
            LogsVM = logsVM;
            ReportsVM = reportsVM;
            SettingsVM = settingsVM;
            AboutVM = aboutVM;

            // Bind ProcessPdf queue and stats to sub-view models
            QueueVM.BindQueue(ProcessPdfVM.Jobs);

            ProcessPdfVM.OnBatchCompleted += summary =>
            {
                ReportsVM.LoadReport(summary);
                NavigateToReports();
            };

            // Set initial active page
            _currentView = DashboardVM;

            NavigateDashboardCommand = new RelayCommand(NavigateToDashboard);
            NavigateProcessPdfCommand = new RelayCommand(NavigateToProcessPdf);
            NavigateQueueCommand = new RelayCommand(NavigateToQueue);
            NavigateLogsCommand = new RelayCommand(NavigateToLogs);
            NavigateReportsCommand = new RelayCommand(NavigateToReports);
            NavigateSettingsCommand = new RelayCommand(NavigateToSettings);
            NavigateAboutCommand = new RelayCommand(NavigateToAbout);
        }

        public void NavigateToDashboard()
        {
            CurrentView = DashboardVM;
            ActivePageName = "Dashboard";
        }

        public void NavigateToProcessPdf()
        {
            CurrentView = ProcessPdfVM;
            ActivePageName = "Process PDFs";
        }

        public void NavigateToQueue()
        {
            CurrentView = QueueVM;
            ActivePageName = "Queue";
        }

        public void NavigateToLogs()
        {
            CurrentView = LogsVM;
            ActivePageName = "Logs";
        }

        public void NavigateToReports()
        {
            CurrentView = ReportsVM;
            ActivePageName = "Reports";
        }

        public void NavigateToSettings()
        {
            CurrentView = SettingsVM;
            ActivePageName = "Settings";
        }

        public void NavigateToAbout()
        {
            CurrentView = AboutVM;
            ActivePageName = "About";
        }
    }
}
