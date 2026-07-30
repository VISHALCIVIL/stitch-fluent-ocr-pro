using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using StitchFluentOcrPro.Logging;

namespace StitchFluentOcrPro.PDF
{
    /// <summary>
    /// High-speed native PDF rendering service using Windows.Data.Pdf APIs.
    /// </summary>
    public class WindowsPdfRendererService : IPdfRendererService
    {
        private readonly ILoggingService _logger;

        public WindowsPdfRendererService(ILoggingService logger)
        {
            _logger = logger;
        }

        public async Task<int> GetPageCountAsync(string pdfPath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Path.GetFullPath(pdfPath));
            PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
            return (int)doc.PageCount;
        }

        public async Task<(double widthPoints, double heightPoints)> GetPageDimensionsAsync(string pdfPath, int pageIndex)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Path.GetFullPath(pdfPath));
            PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
            using PdfPage page = doc.GetPage((uint)pageIndex);
            return (page.Size.Width, page.Size.Height);
        }

        public async Task<MemoryStream> RenderPageToImageStreamAsync(string pdfPath, int pageIndex, int dpi = 300)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Path.GetFullPath(pdfPath));
            PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
            using PdfPage page = doc.GetPage((uint)pageIndex);

            var options = new PdfPageRenderOptions();
            double scale = dpi / 72.0;
            options.DestinationWidth = (uint)Math.Round(page.Size.Width * scale);
            options.DestinationHeight = (uint)Math.Round(page.Size.Height * scale);

            using var inMemStream = new InMemoryRandomAccessStream();
            await page.RenderToStreamAsync(inMemStream, options);

            var memoryStream = new MemoryStream();
            inMemStream.Seek(0);
            await inMemStream.AsStreamForRead().CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
