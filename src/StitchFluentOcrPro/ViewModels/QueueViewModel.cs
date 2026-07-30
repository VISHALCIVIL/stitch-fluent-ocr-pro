using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.ViewModels
{
    public class QueueViewModel : ViewModelBase
    {
        private ObservableCollection<PdfJob> _sourceJobs = new ObservableCollection<PdfJob>();
        private string _searchText = string.Empty;
        private string _selectedFilter = "All";

        public ObservableCollection<PdfJob> FilteredJobs { get; } = new ObservableCollection<PdfJob>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        public ICommand ClearSearchCommand { get; }

        public QueueViewModel()
        {
            ClearSearchCommand = new RelayCommand(() => SearchText = string.Empty);
        }

        public void BindQueue(ObservableCollection<PdfJob> jobs)
        {
            _sourceJobs = jobs;
            _sourceJobs.CollectionChanged += (s, e) => ApplyFilter();
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            FilteredJobs.Clear();
            var query = _sourceJobs.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(j => 
                    j.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    j.RelativeDirectory.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedFilter != "All")
            {
                if (Enum.TryParse<JobStatus>(SelectedFilter, out var status))
                {
                    query = query.Where(j => j.Status == status);
                }
            }

            foreach (var job in query)
            {
                FilteredJobs.Add(job);
            }
        }
    }
}
