using System;
using System.IO;
using System.Text.Json;

namespace StitchFluentOcrPro.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configFilePath;
        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions { WriteIndented = true };

        public AppSettings Settings { get; private set; } = new AppSettings();

        public ConfigurationService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, "StitchFluentOcrPro");
            Directory.CreateDirectory(appFolder);
            _configFilePath = Path.Combine(appFolder, "appsettings.json");
            Load();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    string json = File.ReadAllText(_configFilePath);
                    AppSettings? loaded = JsonSerializer.Deserialize<AppSettings>(json, JsonOpts);
                    if (loaded != null)
                    {
                        Settings = loaded;
                    }
                }
            }
            catch
            {
                // Fall back to default settings on read error
                Settings = new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(Settings, JsonOpts);
                File.WriteAllText(_configFilePath, json);
            }
            catch
            {
                // Ignore save failures in restricted user directories
            }
        }
    }
}
