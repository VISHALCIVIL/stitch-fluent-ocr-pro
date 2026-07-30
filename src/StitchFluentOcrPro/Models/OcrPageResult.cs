using System.Collections.Generic;

namespace StitchFluentOcrPro.Models
{
    /// <summary>
    /// Holds OCR processing output for a single page of a PDF document.
    /// </summary>
    public sealed class OcrPageResult
    {
        public int PageIndex { get; set; }
        public double PageWidthPoints { get; set; }
        public double PageHeightPoints { get; set; }
        public uint ImageWidthPixels { get; set; }
        public uint ImageHeightPixels { get; set; }
        public List<OcrWordInfo> Words { get; set; } = new List<OcrWordInfo>();
        public string ExtractedText { get; set; } = string.Empty;
        public byte[] RenderedImageBytes { get; set; } = System.Array.Empty<byte>();
    }
}
