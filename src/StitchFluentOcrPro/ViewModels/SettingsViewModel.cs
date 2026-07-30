using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StitchFluentOcrPro.Configuration;
using StitchFluentOcrPro.Infrastructure;
using StitchFluentOcrPro.OCR;

namespace StitchFluentOcrPro.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configService;
        private readonly IOcrEngineService _ocrEngine;

        private int _maxParallelWorkers;
        private int _renderDpi;
        private string _selectedLanguage = "en-US";
        private bool _skipExistingFiles;
        private bool _preserveMetadata;
        private string _selectedTheme = "System";

        public ObservableCollection<string> AvailableLanguages { get; } = new ObservableCollection<string>();
        public ObservableCollection<int> DpiOptions { get; } = new ObservableCollection<int> { 150, 200, 300, 400, 600 };
        public ObservableCollection<string> ThemeOptions { get; } = new ObservableCollection<string> { "Light", "Dark", "System" };

        public int MaxCpuCores => Environment.ProcessorCount;

        public int MaxParallelWorkers
        {
            get => _maxParallelWorkers;
            set
            {
                if (SetProperty(ref _maxParallelWorkers, value))
                {
                    _configService.Settings.MaxDegreeOfParallelism = value;
                }
            }
        }

        public int RenderDpi
        {
            get => _renderDpi;
            set
            {
                if (SetProperty(ref _renderDpi, value))
                {
                    _configService.Settings.RenderDpi = value;
                }
            }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    _configService.Settings.SelectedLanguageTag = value;
                }
            }
        }

        public bool SkipExistingFiles
        {
            get => _skipExistingFiles;
            set
            {
                if (SetProperty(ref _skipExistingFiles, value))
                {
                    _configService.Settings.SkipExistingFiles = value;
                }
            }
        }

        public bool PreserveMetadata
        {
            get => _preserveMetadata;
            set
            {
                if (SetProperty(ref _preserveMetadata, value))
                {
                    _configService.Settings.PreserveOriginalMetadata = value;
                }
            }
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                {
                    _configService.Settings.Theme = value;
                }
            }
        }

        public ICommand SaveCommand { get; }

        public SettingsViewModel(IConfigurationService configService, IOcrEngineService ocrEngine)
        {
            _configService = configService;
            _ocrEngine = ocrEngine;

            SaveCommand = new RelayCommand(Save);

            LoadAvailableLanguages();
            LoadSettings();
        }

        private void LoadAvailableLanguages()
        {
            AvailableLanguages.Clear();
            var langs = _ocrEngine.GetAvailableLanguages();
            foreach (var lang in langs)
            {
                AvailableLanguages.Add(lang);
            }

            if (AvailableLanguages.Count == 0)
            {
                AvailableLanguages.Add("en-US");
            }
        }

        private void LoadSettings()
        {
            var s = _configService.Settings;
            MaxParallelWorkers = Math.Clamp(s.MaxDegreeOfParallelism, 1, MaxCpuCores);
            RenderDpi = DpiOptions.Contains(s.RenderDpi) ? s.RenderDpi : 300;
            SkipExistingFiles = s.SkipExistingFiles;
            PreserveMetadata = s.PreserveOriginalMetadata;
            SelectedTheme = ThemeOptions.Contains(s.Theme) ? s.Theme : "System";

            if (AvailableLanguages.Contains(s.SelectedLanguageTag))
            {
                SelectedLanguage = s.SelectedLanguageTag;
            }
            else
            {
                SelectedLanguage = AvailableLanguages.FirstOrDefault() ?? "en-US";
            }
        }

        private void Save()
        {
            _configService.Save();
        }
    }
}
