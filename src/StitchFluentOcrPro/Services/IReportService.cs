using System.Collections.Generic;
using System.Threading.Tasks;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.Services
{
    public interface IReportService
    {
        ReportSummary GenerateSummary(
            string inputFolder, 
            string outputFolder, 
            IReadOnlyList<PdfJob> jobs, 
            System.TimeSpan totalExecutionTime);

        Task ExportJsonReportAsync(string filePath, ReportSummary summary);
        Task ExportCsvReportAsync(string filePath, ReportSummary summary);
    }
}
