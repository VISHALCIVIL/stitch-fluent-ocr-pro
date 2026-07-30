using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
    /// Constructs searchable sandwich PDFs by placing OCR text under original page images (Invisible Searchable Sandwich PDF standard).
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

                        // 1. Place OCR text layer FIRST on canvas
                        foreach (var word in pageResult.Words)
                        {
                            if (string.IsNullOrWhiteSpace(word.Text)) continue;

                            try
                            {
                                var position = new PdfPoint(word.PdfX, word.PdfY);
                                double fontSize = Math.Clamp(word.FontSize, 4.0, 72.0);

                                page.AddText(
                                    word.Text,
                                    fontSize,
                                    position,
                                    font);
                            }
                            catch
                            {
                                try
                                {
                                    string cleanText = Regex.Replace(word.Text, @"[^\u0000-\u007F]+", " ");
                                    if (!string.IsNullOrWhiteSpace(cleanText))
                                    {
                                        var position = new PdfPoint(word.PdfX, word.PdfY);
                                        double fontSize = Math.Clamp(word.FontSize, 4.0, 72.0);
                                        page.AddText(cleanText, fontSize, position, font);
                                    }
                                }
                                catch { }
                            }
                        }

                        // 2. Draw original page image ON TOP of text layer to make text completely invisible visually
                        if (pageResult.RenderedImageBytes != null && pageResult.RenderedImageBytes.Length > 0)
                        {
                            var placement = new PdfRectangle(
                                0, 
                                0, 
                                pageResult.PageWidthPoints, 
                                pageResult.PageHeightPoints);

                            try
                            {
                                page.AddJpeg(pageResult.RenderedImageBytes, placement);
                            }
                            catch
                            {
                                page.AddPng(pageResult.RenderedImageBytes, placement);
                            }
                        }
                    }

                    byte[] pdfBytes = builder.Build();
                    File.WriteAllBytes(outputPath, pdfBytes);
                    _logger.LogInfo($"Successfully created invisible searchable PDF: {outputPath}", "PDF");
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
