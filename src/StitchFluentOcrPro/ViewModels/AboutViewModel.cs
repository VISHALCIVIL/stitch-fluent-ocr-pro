using System;

namespace StitchFluentOcrPro.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string AppVersion => "v1.0.0 (Production)";
        public string TargetFramework => ".NET 8.0 (Windows 10/11 WinRT)";
        public string OcrEngineName => "Windows Native OCR (Windows.Media.Ocr)";
        public string PdfEngineName => "Native Windows.Data.Pdf & PdfPig";
        public string Architecture => "Clean Architecture, MVVM & SOLID";
        public int LogicalCpuCores => Environment.ProcessorCount;
        public string SystemOs => Environment.OSVersion.ToString();
    }
}
