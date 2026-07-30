using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using StitchFluentOcrPro.Logging;

namespace StitchFluentOcrPro.PDF
{
    /// <summary>
    /// High-speed native PDF rendering service using Windows.Data.Pdf APIs with lossless PNG quality.
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
            using FileStream fileStream = File.OpenRead(Path.GetFullPath(pdfPath));
            using IRandomAccessStream winRtStream = fileStream.AsRandomAccessStream();
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(winRtStream);
            return (int)doc.PageCount;
        }

        public async Task<(double widthPoints, double heightPoints)> GetPageDimensionsAsync(string pdfPath, int pageIndex)
        {
            using FileStream fileStream = File.OpenRead(Path.GetFullPath(pdfPath));
            using IRandomAccessStream winRtStream = fileStream.AsRandomAccessStream();
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(winRtStream);
            using PdfPage page = doc.GetPage((uint)pageIndex);
            return (page.Size.Width, page.Size.Height);
        }

        public async Task<MemoryStream> RenderPageToImageStreamAsync(string pdfPath, int pageIndex, int dpi = 300)
        {
            using FileStream fileStream = File.OpenRead(Path.GetFullPath(pdfPath));
            using IRandomAccessStream winRtStream = fileStream.AsRandomAccessStream();
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(winRtStream);
            using PdfPage page = doc.GetPage((uint)pageIndex);

            var options = new PdfPageRenderOptions();
            double scale = dpi / 72.0;
            options.DestinationWidth = (uint)Math.Round(page.Size.Width * scale);
            options.DestinationHeight = (uint)Math.Round(page.Size.Height * scale);

            // Use PNG encoder for 100% lossless image quality and original file size preservation
            options.BitmapEncodingId = BitmapDecoder.PngDecoderId;

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
