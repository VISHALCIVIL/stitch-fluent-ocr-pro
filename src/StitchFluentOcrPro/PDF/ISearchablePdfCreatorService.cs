using System.Collections.Generic;
using System.Threading.Tasks;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.PDF
{
    public interface ISearchablePdfCreatorService
    {
        Task CreateSearchablePdfAsync(string outputPath, List<OcrPageResult> pageResults);
    }
}
