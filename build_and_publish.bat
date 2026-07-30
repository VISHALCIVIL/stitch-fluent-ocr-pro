@echo off
echo [1/2] Restoring and Publishing Stitch Fluent OCR Pro (win-x64 Self-Contained)...
dotnet publish src\StitchFluentOcrPro\StitchFluentOcrPro.csproj -c Release -r win-x64 --self-contained true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ======================================================================
    echo SUCCESS: Published self-contained build to src\StitchFluentOcrPro\publish\
    echo You can now compile Setup.iss in Inno Setup to create the installer.
    echo ======================================================================
) else (
    echo.
    echo ERROR: Build failed. Please check log messages above.
)
pause
