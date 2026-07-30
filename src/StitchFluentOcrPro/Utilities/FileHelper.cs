using System;
using System.IO;

namespace StitchFluentOcrPro.Utilities
{
    public static class FileHelper
    {
        public static string ResolveOutputPath(string rootInputDir, string rootOutputDir, string currentInputFilePath)
        {
            string relativePath = Path.GetRelativePath(rootInputDir, currentInputFilePath);
            string fullOutputPath = Path.Combine(rootOutputDir, relativePath);

            // Prevent overwriting input file directly if input and output folders are identical
            if (string.Equals(Path.GetFullPath(currentInputFilePath), Path.GetFullPath(fullOutputPath), StringComparison.OrdinalIgnoreCase))
            {
                string dir = Path.GetDirectoryName(fullOutputPath) ?? rootOutputDir;
                string filenameWithoutExt = Path.GetFileNameWithoutExtension(fullOutputPath);
                fullOutputPath = Path.Combine(dir, $"{filenameWithoutExt}_searchable.pdf");
            }

            string? parentDirectory = Path.GetDirectoryName(fullOutputPath);
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
            }

            return fullOutputPath;
        }

        public static bool IsPdfFile(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath) && 
                   filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public static long GetFileSizeSafely(string filePath)
        {
            try
            {
                var fi = new FileInfo(filePath);
                return fi.Exists ? fi.Length : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
