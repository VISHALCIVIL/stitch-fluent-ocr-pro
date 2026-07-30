using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using StitchFluentOcrPro.Logging;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.PDF
{
    /// <summary>
    /// Constructs searchable sandwich PDFs by preserving original page images and overlaying calculated OCR text layers.
    /// </summary>
    public class SearchablePdfCreatorService : ISearchablePdfCreatorService
    {
        private readonly ILoggingService _logger;

        public SearchablePdfCreatorService(ILoggingService logger)
        {
            _logger = logger;
        }

        public Task CreateSearchablePdfAsync(string outputPath, List<OcrPageResult> pageResults)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Ensure output folder exists
                    string? dir = Path.GetDirectoryName(outputPath);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using var builder = new PdfDocumentBuilder();
                    PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);

                    foreach (var pageResult in pageResults)
                    {
                        PdfPageBuilder page = builder.AddPage(
                            pageResult.PageWidthPoints, 
                            pageResult.PageHeightPoints);

                        // 1. Draw page image onto PDF background canvas
                        if (pageResult.RenderedImageBytes.Length > 0)
                        {
                            var placement = new PdfRectangle(
                                0, 
                                0, 
                                pageResult.PageWidthPoints, 
                                pageResult.PageHeightPoints);

                            page.AddImage(pageResult.RenderedImageBytes, placement);
                        }

                        // 2. Add invisible OCR text layer matching exact word bounding boxes
                        foreach (var word in pageResult.Words)
                        {
                            if (string.IsNullOrWhiteSpace(word.Text)) continue;

                            try
                            {
                                var position = new PdfPoint(word.PdfX, word.PdfY);
                                double fontSize = Math.Clamp(word.FontSize, 4.0, 72.0);

                                page.AddText(
                                    word.Text,
                                    (decimal)fontSize,
                                    position,
                                    font);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"Failed to place OCR word '{word.Text}' on page {pageResult.PageIndex}: {ex.Message}");
                            }
                        }
                    }

                    byte[] pdfBytes = builder.Build();
                    File.WriteAllBytes(outputPath, pdfBytes);
                    _logger.LogInfo($"Successfully created searchable PDF: {outputPath}", "PDF");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to generate searchable PDF at {outputPath}.", ex, "PDF");
                    throw;
                }
            });
        }
    }
}
