using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.OCR
{
    public interface IOcrEngineService
    {
        IReadOnlyList<string> GetAvailableLanguages();
        Task<OcrPageResult> ProcessImageStreamAsync(
            Stream imageStream, 
            int pageIndex, 
            double pageWidthPoints, 
            double pageHeightPoints, 
            string? languageTag = null);
    }
}
