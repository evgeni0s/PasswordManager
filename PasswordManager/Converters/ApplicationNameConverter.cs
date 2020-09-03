using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace GUI.Converters
{
    public class ApplicationNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (string.IsNullOrEmpty(strValue))
            {
                return "**All applications**";
            }
            return strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (strValue == "**All applications**")
            {
                return string.Empty;
            }
            return strValue;
        }
    }
}
