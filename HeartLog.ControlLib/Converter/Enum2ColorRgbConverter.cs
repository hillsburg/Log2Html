using System.Windows.Data;
using System.Windows.Media;
using HeartLog.ControlLib.Logger.Enum;

namespace HeartLog.ControlLib.Converter
{
    public class Enum2ColorRgbConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            LogLevel colorEnum = (LogLevel)value;

            switch (colorEnum)
            {
                case LogLevel.Error:
                    return Colors.Red;
                case LogLevel.Warning:
                    return Colors.DarkOrange;
                case LogLevel.Info:
                    return Colors.Black;
                default:
                    throw new InvalidOperationException("Unknown color");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color color = (Color)value;

            if (color == Colors.Red)
                return LogLevel.Error;
            else if (color == Colors.DarkOrange)
                return LogLevel.Warning;
            else if (color == Colors.Black)
                return LogLevel.Info;
            else
                throw new InvalidOperationException("Unknown color");
        }
    }
}
