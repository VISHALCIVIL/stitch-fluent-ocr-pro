using StitchFluentOcrPro.Configuration;
using Xunit;

namespace StitchFluentOcrPro.Tests
{
    public class ConfigurationServiceTests
    {
        [Fact]
        public void AppSettings_DefaultValues_AreCorrect()
        {
            var settings = new AppSettings();

            Assert.Equal(300, settings.RenderDpi);
            Assert.True(settings.SkipExistingFiles);
            Assert.True(settings.PreserveOriginalMetadata);
            Assert.Equal("System", settings.Theme);
            Assert.Equal("en-US", settings.SelectedLanguageTag);
        }
    }
}
