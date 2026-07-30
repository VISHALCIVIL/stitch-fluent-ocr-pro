using System;
using StitchFluentOcrPro.OCR;
using Xunit;

namespace StitchFluentOcrPro.Tests
{
    public class OcrCoordinateConverterTests
    {
        [Fact]
        public void ConvertPixelToPdfCoordinates_ValidInputs_CalculatesCorrectPdfPoints()
        {
            // Scenario:
            // Standard Letter page size in PDF points: 612 x 792 (8.5" x 11" at 72 points/inch)
            // Rendered Image size at 300 DPI: 2550 x 3300 pixels
            // Word Bounding Box in Image space: X=255, Y=330, Width=255, Height=66

            double pageWidthPoints = 612.0;
            double pageHeightPoints = 792.0;
            uint imageWidthPixels = 2550;
            uint imageHeightPixels = 3300;

            double pixelX = 255.0;
            double pixelY = 330.0;
            double pixelWidth = 255.0;
            double pixelHeight = 66.0;

            var result = OcrCoordinateConverter.ConvertPixelToPdfCoordinates(
                "SampleText",
                pixelX,
                pixelY,
                pixelWidth,
                pixelHeight,
                imageWidthPixels,
                imageHeightPixels,
                pageWidthPoints,
                pageHeightPoints,
                0.95f);

            // Scale X = 612 / 2550 = 0.24
            // Scale Y = 792 / 3300 = 0.24
            // PdfX = 255 * 0.24 = 61.2
            // PdfY = (3300 - (330 + 66)) * 0.24 = (3300 - 396) * 0.24 = 2904 * 0.24 = 696.96
            // PdfWidth = 255 * 0.24 = 61.2
            // PdfHeight = 66 * 0.24 = 15.84

            Assert.Equal("SampleText", result.Text);
            Assert.Equal(61.2, result.PdfX, 2);
            Assert.Equal(696.96, result.PdfY, 2);
            Assert.Equal(61.2, result.PdfWidth, 2);
            Assert.Equal(15.84, result.PdfHeight, 2);
            Assert.Equal(15.84, result.FontSize, 2);
        }

        [Fact]
        public void ConvertPixelToPdfCoordinates_ZeroDimensions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                OcrCoordinateConverter.ConvertPixelToPdfCoordinates(
                    "Test", 10, 10, 50, 20, 0, 100, 612, 792));
        }
    }
}
