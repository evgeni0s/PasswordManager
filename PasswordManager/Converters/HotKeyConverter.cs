using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace GUI.Converters
{
    public class HotKeyConverter : IValueConverter
    {
        // From Model to GUI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var password = value as string;
            var list = new List<string>();
            while (!string.IsNullOrEmpty(password))
            {
                int length = 0;
                var nextKey = GetNextKey(password, out length);
                list.Add(nextKey);
                password = password.Substring(length);
            }
            return string.Join(string.Empty, list);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var password = value as string;
            var list = new List<string>();
            while (!string.IsNullOrEmpty(password))
            {
                int length = 0;
                var nextKey = GetNextKeyWithoutBrakets(password, out length);
                list.Add(nextKey);
                password = password.Substring(length);
            }
            return string.Join(string.Empty, list);
        }

        public string GetNextKey(string key, out int length)
        {
            for (int i = 0; i < Constants.SpecialKeyDictionary.Length; i++)
            {
                var keyFound = key.ToLower().StartsWith(Constants.SpecialKeyDictionary[i]);
                if (keyFound)
                {
                    length = Constants.SpecialKeyDictionary[i].Length;
                    return Constants.SpecialKeyDictionaryWithoutBrakets[i];
                }
            }
            length = 1;
            return key[0].ToString();
        }

        public static string GetNextKeyWithoutBrakets(string key, out int length)
        {
            for (int i = 0; i < Constants.SpecialKeyDictionaryWithoutBrakets.Length; i++)
            {
                var keyFound = key.ToLower().StartsWith(Constants.SpecialKeyDictionaryWithoutBrakets[i]);
                if (keyFound)
                {
                    length = Constants.SpecialKeyDictionaryWithoutBrakets[i].Length;
                    return Constants.SpecialKeyDictionary[i];
                }
            }
            length = 1;
            return key[0].ToString();
        }
    }
}
