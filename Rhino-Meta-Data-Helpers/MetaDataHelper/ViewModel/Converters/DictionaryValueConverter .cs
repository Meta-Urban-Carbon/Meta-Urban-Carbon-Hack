using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;


namespace MetaDataHelper
{
    public class DictionaryValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dictionary<string, string> dictionary)
            {
                var key = parameter as string;
                if (key != null && dictionary.ContainsKey(key))
                {
                    return dictionary[key];
                }
            } else if (value is String)
            {
                return value;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Implement if needed for two-way binding
            throw new NotImplementedException();
        }
    }
}
