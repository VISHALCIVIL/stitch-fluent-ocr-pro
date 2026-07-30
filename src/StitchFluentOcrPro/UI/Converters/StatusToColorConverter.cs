using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using StitchFluentOcrPro.Models;

namespace StitchFluentOcrPro.UI.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JobStatus status)
            {
                return status switch
                {
                    JobStatus.Queued => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#717783")),
                    JobStatus.Processing => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4")),
                    JobStatus.Paused => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BC5B00")),
                    JobStatus.Completed => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#107C41")),
                    JobStatus.Failed => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BA1A1A")),
                    JobStatus.Skipped => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#656464")),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
