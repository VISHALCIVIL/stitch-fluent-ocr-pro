namespace StitchFluentOcrPro.Configuration
{
    /// <summary>
    /// Application configuration options saved and loaded from appsettings.json.
    /// </summary>
    public class AppSettings
    {
        public string InputFolder { get; set; } = string.Empty;
        public string OutputFolder { get; set; } = string.Empty;
        public int MaxDegreeOfParallelism { get; set; } = System.Environment.ProcessorCount;
        public int RenderDpi { get; set; } = 300; // 300 DPI default for 100% original image quality and file size
        public string SelectedLanguageTag { get; set; } = "en-US";
        public bool SkipExistingFiles { get; set; } = true;
        public bool PreserveOriginalMetadata { get; set; } = true;
        public string Theme { get; set; } = "System"; // Light, Dark, System
        public bool AutoScrollLogs { get; set; } = true;
    }
}
