using System.Collections.Generic;
using System.Threading.Tasks;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.Services
{
    public interface IFolderScannerService
    {
        Task<List<PdfJob>> ScanFolderAsync(string inputFolder, string outputFolder, bool skipExisting = true);
    }
}
