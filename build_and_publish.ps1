# =====================================================================
# Stitch Fluent OCR Pro - Build & Publish Script for Windows
# =====================================================================

Write-Host "[1/3] Restoring NuGet Packages..." -ForegroundColor Cyan
dotnet restore

Write-Host "[2/3] Building & Publishing Self-Contained win-x64 Release..." -ForegroundColor Cyan
dotnet publish src/StitchFluentOcrPro/StitchFluentOcrPro.csproj -c Release -r win-x64 --self-contained true

if ($LASTEXITCODE -eq 0) {
    Write-Host "[SUCCESS] Self-contained application created successfully in:" -ForegroundColor Green
    Write-Host "          src\StitchFluentOcrPro\publish\" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "[3/3] You can now compile 'Setup.iss' using Inno Setup 6 to generate the installer." -ForegroundColor Cyan
} else {
    Write-Host "[ERROR] Publish failed. Please check build errors above." -ForegroundColor Red
}
