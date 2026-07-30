using System;
using System.Globalization;
using System.Windows.Data;
using StitchFluentOcrPro.Utilities;

namespace StitchFluentOcrPro.UI.Converters
{
    public class BytesToHumanReadableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                return FileSizeFormatter.FormatBytes(bytes);
            }
            return "0 B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
