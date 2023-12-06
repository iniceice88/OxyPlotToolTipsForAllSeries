using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace OxyplotToolTipsForAllLines.Converters;

public class SeriesColorToBrushConverter : IValueConverter
{
    public Brush DefaultBrush { get; set; } = Brushes.Black;

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return DefaultBrush;

        if (value is not Series series)
            return DefaultBrush;

        if (series is LineSeries lineSeries)
            return lineSeries.ActualColor.ToBrush() ?? DefaultBrush;

        return DefaultBrush;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}