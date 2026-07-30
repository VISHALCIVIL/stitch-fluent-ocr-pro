# Stitch Fluent OCR Pro (.NET 8 WPF Desktop Application)

**Stitch Fluent OCR Pro** is an enterprise-grade, offline searchable PDF conversion desktop application built with **C# (.NET 8)**, **WPF**, and modern **Windows 11 Fluent Design**.

It utilizes the native **Windows OCR API (`Windows.Media.Ocr`)** and **Windows PDF Rendering APIs (`Windows.Data.Pdf`)** to convert scanned image-only PDFs into fully searchable PDFs with invisible text layers while preserving high-resolution page rendering, page dimensions, rotation, bookmarks, and relative subfolder directory structures.

---

## 🍎 Developing on macOS & Building for Windows

Since WPF (Windows Presentation Foundation) and WinRT Windows SDK APIs (`Windows.Media.Ocr`) are Windows-native platforms, the **XAML compiler and Windows SDK runtime require a Windows environment to compile the final `.exe` binaries**.

### Recommended Options to Generate the `.exe` Installer:

#### 1. Automated GitHub Actions CI (Free & Automatic)
We have included a pre-configured GitHub Actions workflow: [.github/workflows/build-windows.yml](file:///.github/workflows/build-windows.yml).
- Simply push this repository to GitHub.
- GitHub's cloud Windows runner (`windows-latest`) will automatically run `dotnet publish` and Inno Setup.
- Download `StitchFluentOcrPro-Setup-Installer.exe` directly from GitHub Actions **Artifacts**!

#### 2. Building on a Windows Machine or VM (Parallels / UTM / Dual Boot)
1. Copy/Zip the repository folder onto any Windows PC or Windows VM.
2. Double-click `build_and_publish.bat` (or run `.\build_and_publish.ps1`).
3. Compile `Setup.iss` in Inno Setup 6 to generate `StitchFluentOcrPro_Setup_v1.0.0.exe`.

---

## 🌟 Key Features

- **100% Offline Engine**: Uses native Windows WinRT APIs (`Windows.Media.Ocr`, `Windows.Graphics.Imaging`, `Windows.Data.Pdf`). Zero dependencies on Tesseract, OCRmyPDF, or cloud services.
- **Searchable Sandwich PDFs**: Injects invisible text layers (`TextRenderingMode.NeitherFillNorStroke`) matching standard PDF specifications (compatible with Adobe Acrobat, Microsoft Edge, Google Chrome, macOS Preview).
- **High Precision Coordinate Math**: Converts bounding boxes from image pixel space (Top-Left origin) into 72 DPI PDF point space (Bottom-Left origin).
- **Extreme Scale Performance**: Process thousands of pages and PDFs larger than 10 GB with zero memory leaks via stream-based page chunking and deterministic GC sweeps.
- **Parallel Multi-Core Execution**: Fully utilizes all CPU logical cores via `Parallel.ForEachAsync` with customizable concurrency sliders.
- **Pause / Resume / Cancel Controls**: Thread-safe execution pause and cancellation token integration.
- **Folder Recursion**: Preserves identical input subfolder structures in the output destination folder.
- **Error Tolerance**: Continues processing remaining files if an individual file fails, detailing errors in live logs and post-run reports.
- **7 Fluent Navigation Pages**: Dashboard, Process PDFs, Queue, Logs, Reports, Settings, and About.
- **Real-Time Telemetry**: Live Pages per Minute (PPM), CPU % gauge, Memory MB gauge, active workers counter, and ETA calculations.

---

## 🏗️ Project Architecture

```
StitchFluentOcrPro/
├── StitchFluentOcrPro.sln
├── .github/workflows/
│   └── build-windows.yml                   # Automated GitHub Actions Cloud Build Workflow
├── build_and_publish.bat                   # 1-Click Windows Batch Publish Script
├── build_and_publish.ps1                   # PowerShell Build & Publish Script
├── Setup.iss                               # Inno Setup 6 Installer Script (Root)
├── installer/
│   └── Setup.iss                           # Inno Setup 6 Installer Script
├── src/
│   └── StitchFluentOcrPro/
│       ├── StitchFluentOcrPro.csproj        # Configured for win-x64 Self-Contained Release
│       ├── App.xaml & App.xaml.cs          # DI registration & App initialization
│       ├── MainWindow.xaml & .cs           # Main Shell Window with Left Nav Rail
│       ├── Properties/PublishProfiles/
│       │   └── FolderProfile.pubxml         # Visual Studio & CLI Publish Profile
│       ├── Configuration/                  # Settings persistence (appsettings.json)
│       ├── Infrastructure/                 # PauseTokenSource, RelayCommand, AsyncRelayCommand
│       ├── Logging/                        # Thread-safe logger & UI event sink
│       ├── Models/                         # Domain DTOs (PdfJob, OcrWordInfo, ProcessingStats)
│       ├── OCR/                            # WindowsOcrEngineService & OcrCoordinateConverter
│       ├── PDF/                            # WindowsPdfRendererService & SearchablePdfCreatorService
│       ├── Services/                       # FolderScannerService, PdfBatchProcessor, SystemMetricsService, ReportService
│       ├── Utilities/                      # TimeFormatter, FileSizeFormatter, FileHelper
│       ├── ViewModels/                     # MainViewModel, DashboardViewModel, ProcessPdfViewModel, QueueViewModel, etc.
│       └── UI/                             # Converters, Styles, Views (7 pages)
└── tests/
    └── StitchFluentOcrPro.Tests/           # xUnit Unit Test suite
```
