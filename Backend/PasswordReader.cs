using Keystroke.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend
{
    public static class PasswordReader
    {
        private static List<KeyCode> notRecordableKeys = new List<KeyCode> { KeyCode.Shift, KeyCode.ShiftKey, KeyCode.LShiftKey, KeyCode.RShiftKey, KeyCode.CapsLock };

        public static SettingsModel NewRow(KeyPressed key)
        {
            var row = new SettingsModel();
            row.ApplicationName = key.CurrentWindow;
            row.Hotkey = "Auto";
            return row;
        }
        public static SettingsModel NewEmptyRow()
        {
            var row = new SettingsModel();
            row.ApplicationName = string.Empty;
            row.Hotkey = "Auto";
            return row;
        }

        public static void RecordKeyStroke(this SettingsModel row, KeyPressed key)
        {
            if (row == null
                // Do not record shift key stroke, because letters are already processed as shifted 
                || notRecordableKeys.Contains(key.KeyCode))
            {
                return;
            }

            if (row.Password == null)
            {
                row.Password = new List<string>();
            }

            row.Password.Add(key.ToString());
        }
    }
}
