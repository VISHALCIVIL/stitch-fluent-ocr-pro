using System;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Services;
using Xunit;

namespace StitchFluentOcrPro.Tests
{
    public class SystemMetricsServiceTests
    {
        [Fact]
        public void CalculatePpm_ValidElapsedAndPages_ReturnsCorrectThroughput()
        {
            var logger = new LoggingService();
            using var metricsService = new SystemMetricsService();

            int pages = 120;
            TimeSpan elapsed = TimeSpan.FromMinutes(2.0);

            double ppm = metricsService.CalculatePpm(pages, elapsed);

            Assert.Equal(60.0, ppm, 1);
        }

        [Fact]
        public void EstimateRemainingTime_ValidRemainingPagesAndPpm_ReturnsCorrectDuration()
        {
            var logger = new LoggingService();
            using var metricsService = new SystemMetricsService();

            int remainingPages = 300;
            double ppm = 60.0;

            TimeSpan remainingTime = metricsService.EstimateRemainingTime(remainingPages, ppm);

            Assert.Equal(5.0, remainingTime.TotalMinutes, 1);
        }
    }
}
