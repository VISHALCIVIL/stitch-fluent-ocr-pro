using System.IO;
using System.Threading.Tasks;

namespace StitchFluentOcrPro.PDF
{
    public interface IPdfRendererService
    {
        Task<int> GetPageCountAsync(string pdfPath);
        Task<(double widthPoints, double heightPoints)> GetPageDimensionsAsync(string pdfPath, int pageIndex);
        Task<MemoryStream> RenderPageToImageStreamAsync(string pdfPath, int pageIndex, int dpi = 300);
    }
}
