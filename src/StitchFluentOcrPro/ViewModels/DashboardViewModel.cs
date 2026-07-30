using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private ProcessingStats _stats = new ProcessingStats();

        public ProcessingStats Stats
        {
            get => _stats;
            set => SetProperty(ref _stats, value);
        }

        public void UpdateStats(ProcessingStats stats)
        {
            Stats = stats;
        }
    }
}
