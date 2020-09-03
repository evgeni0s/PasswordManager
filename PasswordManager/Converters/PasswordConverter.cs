using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace GUI.Converters
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as List<string>;
            if (list != null)
            {
                return string.Join(string.Empty, list);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var password = value as string;
            var list = new List<string>();
            while (!string.IsNullOrEmpty(password))
            {
                var nextKey = GetNextKey(password);
                list.Add(nextKey);
                password = password.Substring(nextKey.Length);
            }
            return list;
        }

        private string GetNextKey(string key)
        {
            for (int i = 0; i < Constants.SpecialKeyDictionary[i].Length; i++)
            {
                var keyFound = key.StartsWith(Constants.SpecialKeyDictionary[i]);
                if (keyFound)
                {
                    return Constants.SpecialKeyDictionary[i];
                }
            }
            return key[0].ToString();
        }
    }
}
