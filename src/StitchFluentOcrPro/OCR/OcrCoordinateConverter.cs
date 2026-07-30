using System;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.OCR
{
    /// <summary>
    /// Converts OCR word bounding boxes from image pixel space (Top-Left origin) 
    /// into standard PDF point space (Bottom-Left origin, 72 points per inch).
    /// </summary>
    public static class OcrCoordinateConverter
    {
        public static OcrWordInfo ConvertPixelToPdfCoordinates(
            string text,
            double pixelX,
            double pixelY,
            double pixelWidth,
            double pixelHeight,
            uint imageWidthPixels,
            uint imageHeightPixels,
            double pageWidthPoints,
            double pageHeightPoints,
            float confidence = 1.0f)
        {
            if (imageWidthPixels == 0 || imageHeightPixels == 0)
            {
                throw new ArgumentException("Image dimensions must be greater than zero.");
            }

            if (pageWidthPoints <= 0 || pageHeightPoints <= 0)
            {
                throw new ArgumentException("Page point dimensions must be greater than zero.");
            }

            double scaleX = pageWidthPoints / imageWidthPixels;
            double scaleY = pageHeightPoints / imageHeightPixels;

            double pdfX = pixelX * scaleX;
            double pdfY = (imageHeightPixels - (pixelY + pixelHeight)) * scaleY;
            double pdfWidth = pixelWidth * scaleX;
            double pdfHeight = pixelHeight * scaleY;

            // Ensure non-zero font size
            double fontSize = Math.Max(1.0, pdfHeight);

            return new OcrWordInfo
            {
                Text = text,
                PixelX = pixelX,
                PixelY = pixelY,
                PixelWidth = pixelWidth,
                PixelHeight = pixelHeight,
                PdfX = Math.Max(0, pdfX),
                PdfY = Math.Max(0, pdfY),
                PdfWidth = Math.Max(1.0, pdfWidth),
                PdfHeight = Math.Max(1.0, pdfHeight),
                FontSize = fontSize,
                Confidence = confidence
            };
        }
    }
}
