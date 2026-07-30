namespace StitchFluentOcrPro.Configuration
{
    public interface IConfigurationService
    {
        AppSettings Settings { get; }
        void Load();
        void Save();
    }
}
