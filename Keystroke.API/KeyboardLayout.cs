using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Keystroke.API
{
    public class KeyboardLayout
    {
        internal CultureInfo GetCurrentKeyboardLayout()
        {
            try
            {
                var foregroundWindow = User32.GetForegroundWindow();
                var foregroundWindowIntPtr = (IntPtr)foregroundWindow;
                uint foregroundProcess = User32.GetWindowThreadProcessId(foregroundWindowIntPtr, IntPtr.Zero);
                int keyboardLayout = User32.GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
                return new CultureInfo(keyboardLayout);
            }
            catch
            {
                return new CultureInfo(1033); // Assume English if something went wrong.
            }
        }
    }
}
