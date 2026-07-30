using System.IO;
using System.Threading.Tasks;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Services;
using StitchFluentOcrPro.Utilities;
using Xunit;

namespace StitchFluentOcrPro.Tests
{
    public class FolderScannerServiceTests
    {
        [Fact]
        public void FileHelper_ResolveOutputPath_PreservesRelativeDirectoryStructure()
        {
            string rootInputDir = Path.Combine("C:", "ScannedDocs");
            string rootOutputDir = Path.Combine("C:", "ProcessedDocs");
            string currentInputFilePath = Path.Combine("C:", "ScannedDocs", "Finance", "2024", "Invoice.pdf");

            string expectedOutput = Path.Combine("C:", "ProcessedDocs", "Finance", "2024", "Invoice.pdf");

            string actualOutput = FileHelper.ResolveOutputPath(rootInputDir, rootOutputDir, currentInputFilePath);

            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public async Task FolderScannerService_NonExistentFolder_ReturnsEmptyList()
        {
            var logger = new LoggingService();
            var scanner = new FolderScannerService(logger);

            var jobs = await scanner.ScanFolderAsync(@"C:\NonExistentTestFolder_12345", @"C:\Output");

            Assert.NotNull(jobs);
            Assert.Empty(jobs);
        }
    }
}
