using System;

namespace StitchFluentOcrPro.Utilities
{
    public static class TimeFormatter
    {
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                return "00:00:00";
            }

            if (timeSpan.TotalDays >= 1)
            {
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours:D2}h {timeSpan.Minutes:D2}m";
            }

            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        public static string FormatPpm(double ppm)
        {
            if (double.IsNaN(ppm) || double.IsInfinity(ppm) || ppm < 0)
            {
                return "0.0 PPM";
            }
            return $"{ppm:F1} PPM";
        }
    }
}
