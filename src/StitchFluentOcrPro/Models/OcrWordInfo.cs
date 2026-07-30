namespace StitchFluentOcrPro.Models
{
    /// <summary>
    /// Holds OCR word recognition data including recognized text and spatial coordinates in both image pixel space and PDF point space.
    /// </summary>
    public sealed class OcrWordInfo
    {
        public string Text { get; set; } = string.Empty;
        
        // Image pixel space (Top-Left origin)
        public double PixelX { get; set; }
        public double PixelY { get; set; }
        public double PixelWidth { get; set; }
        public double PixelHeight { get; set; }

        // PDF point space (Bottom-Left origin, 72 DPI)
        public double PdfX { get; set; }
        public double PdfY { get; set; }
        public double PdfWidth { get; set; }
        public double PdfHeight { get; set; }
        public double FontSize { get; set; }

        public float Confidence { get; set; } = 1.0f;
    }
}
