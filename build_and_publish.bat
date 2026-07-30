@echo off
cd /d "%~dp0"
echo ===================================================
echo Stitch Fluent OCR Pro - Build & Publish Script
echo ===================================================
echo.

echo Restoring dependencies and publishing self-contained executable...
dotnet publish "src/StitchFluentOcrPro/StitchFluentOcrPro.csproj" -c Release -r win-x64 --self-contained true -o "%~dp0src\StitchFluentOcrPro\publish"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build or Publish failed! Please check error output above.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ===================================================
echo [SUCCESS] Application published successfully!
echo Published output location:
echo %~dp0src\StitchFluentOcrPro\publish\
echo ===================================================
echo.

if exist "%~dp0src\StitchFluentOcrPro\publish" (
    echo Opening published folder in File Explorer...
    explorer "%~dp0src\StitchFluentOcrPro\publish"
) else (
    echo [WARNING] Published folder was not found at %~dp0src\StitchFluentOcrPro\publish
)

pause
