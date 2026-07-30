using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.OCR
{
    /// <summary>
    /// High-performance offline OCR engine wrapper over native Windows.Media.Ocr APIs.
    /// Encodes page images to compact JPEG streams to preserve original small file sizes.
    /// </summary>
    public class WindowsOcrEngineService : IOcrEngineService
    {
        private readonly ILoggingService _logger;
        private const uint MaxOcrDimension = 2560; // Windows.Media.Ocr MaxImageDimension constraint (2600)

        public WindowsOcrEngineService(ILoggingService logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<string> GetAvailableLanguages()
        {
            try
            {
                var languages = OcrEngine.AvailableRecognizerLanguages;
                return languages.Select(l => l.LanguageTag).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to retrieve Windows OCR recognizer languages.", ex, "OCR");
                return new List<string> { "en-US" };
            }
        }

        public async Task<OcrPageResult> ProcessImageStreamAsync(
            Stream imageStream,
            int pageIndex,
            double pageWidthPoints,
            double pageHeightPoints,
            string? languageTag = null)
        {
            var pageResult = new OcrPageResult
            {
                PageIndex = pageIndex,
                PageWidthPoints = pageWidthPoints,
                PageHeightPoints = pageHeightPoints
            };

            if (imageStream.CanSeek)
            {
                imageStream.Position = 0;
            }

            using var rawMs = new MemoryStream();
            await imageStream.CopyToAsync(rawMs);
            byte[] rawBytes = rawMs.ToArray();

            // Create WinRT random access stream for image decoding
            using IRandomAccessStream winRtStream = new InMemoryRandomAccessStream();
            await winRtStream.WriteAsync(rawBytes.AsBuffer());
            winRtStream.Seek(0);

            // Decode image into SoftwareBitmap
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(winRtStream);
            uint origWidth = decoder.PixelWidth;
            uint origHeight = decoder.PixelHeight;
            pageResult.ImageWidthPixels = origWidth;
            pageResult.ImageHeightPixels = origHeight;

            // Check if image exceeds Windows OCR MaxImageDimension limit (2600px)
            SoftwareBitmap softwareBitmap;
            uint ocrWidth = origWidth;
            uint ocrHeight = origHeight;

            if (origWidth > MaxOcrDimension || origHeight > MaxOcrDimension)
            {
                double scale = Math.Min((double)MaxOcrDimension / origWidth, (double)MaxOcrDimension / origHeight);
                ocrWidth = (uint)Math.Max(1, Math.Round(origWidth * scale));
                ocrHeight = (uint)Math.Max(1, Math.Round(origHeight * scale));

                var transform = new BitmapTransform
                {
                    ScaledWidth = ocrWidth,
                    ScaledHeight = ocrHeight,
                    InterpolationMode = BitmapInterpolationMode.Linear
                };

                softwareBitmap = await decoder.GetSoftwareBitmapAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);
            }
            else
            {
                softwareBitmap = await decoder.GetSoftwareBitmapAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied);
            }

            using (softwareBitmap)
            {
                // Re-encode to compact high-quality JPEG to keep output PDF file size small (~300KB)
                using var jpegStream = new InMemoryRandomAccessStream();
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, jpegStream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();

                using var msJpeg = new MemoryStream();
                jpegStream.Seek(0);
                await jpegStream.AsStreamForRead().CopyToAsync(msJpeg);
                pageResult.RenderedImageBytes = msJpeg.ToArray();

                // Initialize Windows OcrEngine
                OcrEngine? engine = null;
                if (!string.IsNullOrWhiteSpace(languageTag))
                {
                    try
                    {
                        var lang = new Language(languageTag);
                        if (OcrEngine.IsLanguageSupported(lang))
                        {
                            engine = OcrEngine.TryCreateFromLanguage(lang);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Could not load OCR engine for language '{languageTag}': {ex.Message}");
                    }
                }

                engine ??= OcrEngine.TryCreateFromUserProfileLanguages();
                if (engine == null)
                {
                    var supported = OcrEngine.AvailableRecognizerLanguages.FirstOrDefault();
                    if (supported != null)
                    {
                        engine = OcrEngine.TryCreateFromLanguage(supported);
                    }
                }

                if (engine == null)
                {
                    throw new InvalidOperationException("No suitable Windows OCR language engine is available on this system.");
                }

                // Perform OCR Recognition
                OcrResult ocrResult = await engine.RecognizeAsync(softwareBitmap);

                var textBuilder = new StringBuilder();
                var wordList = new List<OcrWordInfo>();

                foreach (OcrLine line in ocrResult.Lines)
                {
                    textBuilder.AppendLine(line.Text);

                    foreach (OcrWord word in line.Words)
                    {
                        var rect = word.BoundingRect;
                        var wordInfo = OcrCoordinateConverter.ConvertPixelToPdfCoordinates(
                            word.Text,
                            rect.X,
                            rect.Y,
                            rect.Width,
                            rect.Height,
                            ocrWidth,
                            ocrHeight,
                            pageWidthPoints,
                            pageHeightPoints,
                            1.0f);

                        wordList.Add(wordInfo);
                    }
                }

                pageResult.Words = wordList;
                pageResult.ExtractedText = textBuilder.ToString();

                _logger.LogInfo($"[OCR] Page {pageIndex + 1}: Recognized {wordList.Count} words.", "OCR");
                return pageResult;
            }
        }
    }
}
