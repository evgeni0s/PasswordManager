using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUI
{
    public static class Constants
    {
        public static string[] SpecialKeyDictionary = new[]
        {
            "<F1>", "<F2>", "<F3>", "<F4>", "<F5>", "<F6>", "<F7>", "<F8>", "<F9>",
             "<F10>", "<F11>", "<F12>", "<print screen>", "<scroll>", "<pause>", "<insert>", "<delete>", "<end>",
              "<page up>", "<page down>", "<esc>", "<numlock>", "<tab>", "<backspace>", "<enter>", " ", "<left>",
            "<right>", "<up>", "<down>", "<alt>", "<win>", "<capsLock>", "<ctrl>", "<shift>", "<volumnDown>",
            "<volumeUp>", "<volumeMute>", "\\", "\""
        };

        public static string[] SpecialKeyDictionaryWithoutBrakets { get; }
        = SpecialKeyDictionary.Select(RemoveBrakets).ToArray();

        private static string RemoveBrakets(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace("<", string.Empty).Replace(">", string.Empty);
        }
    }
}
