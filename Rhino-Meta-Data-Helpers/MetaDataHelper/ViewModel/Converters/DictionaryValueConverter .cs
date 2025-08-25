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

        //TODO this will cause a error if the user tries to edit the value in table. 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string key)
            {
                // Return a new dictionary with the updated value
                // Assuming 'value' is the new value for the attribute corresponding to 'key'
                return new Dictionary<string, string> { { key, stringValue } };
            }

            // Return the original value if the conversion is not applicable
            return value;
        }

    }
}
