using System;
using System.Globalization;
using System.Windows.Data;

namespace MetaDataHelper
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return double.TryParse(stringValue, NumberStyles.Any, culture, out double result) ? result : 0.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue.ToString(culture);
            }
            return "0.0";
        }
    }
}
