using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Keystroke.API
{
    public class KeyboardListener : IDisposable
	{
		private IntPtr globalKeyboardHookId;
		private IntPtr currentModuleId;
		private const int WH_KEYBOARD_LL = 13;
		private const int WH_MOUSE_LL = 14;
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYDOWN = 0x104;
		private const int WM_SYSKEYUP = 0x105;
		private User32.LowLevelHook HookKeyboardDelegate; //We need to have this delegate as a private field so the GC doesn't collect it
		private Func<KeyPressed, bool> keyPressedCallback;
		private Func<KeyPressed, bool> keyReleasedCallback;

		public KeyboardListener()
		{
			Process currentProcess = Process.GetCurrentProcess();
			ProcessModule currentModudle = currentProcess.MainModule;
			this.currentModuleId = User32.GetModuleHandle(currentModudle.ModuleName);
		}

		public void CreateKeyboardHook(Func<KeyPressed, bool> keyPressedCallback, Func<KeyPressed, bool> keyReleasedCallback)
		{
			this.keyPressedCallback = keyPressedCallback;
			this.keyReleasedCallback = keyReleasedCallback;
			this.HookKeyboardDelegate = HookKeyboardCallbackImplementation;
			this.globalKeyboardHookId = User32.SetWindowsHookEx(WH_KEYBOARD_LL, this.HookKeyboardDelegate, this.currentModuleId, 0);
		}

		private IntPtr HookKeyboardCallbackImplementation(int nCode, IntPtr wParam, IntPtr lParam)
		{
			int wParamAsInt = wParam.ToInt32();
			bool handled = false;

			if (nCode >= 0 && (wParamAsInt == WM_KEYUP || wParamAsInt == WM_SYSKEYUP))
			{
				handled = KeyParserUp(wParam, lParam);
			}

			if (nCode >= 0 && (wParamAsInt == WM_KEYDOWN || wParamAsInt == WM_SYSKEYDOWN))
			{
				bool shiftPressed = false;
				bool capsLockActive = false;

				var shiftKeyState = User32.GetAsyncKeyState(KeyCode.ShiftKey);
				if (FirstBitIsTurnedOn(shiftKeyState))
					shiftPressed = true;

				//We need to use GetKeyState to verify if CapsLock is "TOGGLED" 
				//because GetAsyncKeyState only verifies if it is "PRESSED" at the moment
				if (User32.GetKeyState(KeyCode.Capital) == 1)
					capsLockActive = true;

				handled = KeyParser(wParam, lParam, shiftPressed, capsLockActive);
			}

			//Chain to the next hook. Otherwise other applications that 
			//are listening to this hook will not get notificied
			if (handled)
			{
				return new IntPtr(1); // this blocks input to original window
			}
			else
			{
				return User32.CallNextHookEx(globalKeyboardHookId, nCode, wParam, lParam); // Allow keysproke to reach original window
			}
		}

		private bool FirstBitIsTurnedOn(short value)
		{
			//0x8000 == 1000 0000 0000 0000			
			return Convert.ToBoolean(value & 0x8000);
		}

		private bool KeyParser(IntPtr wParam, IntPtr lParam, bool shiftPressed, bool capsLockPressed)
		{
			var keyValue = (KeyCode)Marshal.ReadInt32(lParam);

			var keyboardLayout = new KeyboardLayout().GetCurrentKeyboardLayout();
			var windowTitle = new Window().CurrentWindowTitle();

			var key = new KeyPressed(keyValue, shiftPressed, capsLockPressed, windowTitle, keyboardLayout.ToString());

			return keyPressedCallback.Invoke(key);
		}

		private bool KeyParserUp(IntPtr wParam, IntPtr lParam)
		{
			bool shiftPressed = false;
			bool capsLockActive = false;
			var shiftKeyState = User32.GetAsyncKeyState(KeyCode.ShiftKey);
			if (FirstBitIsTurnedOn(shiftKeyState))
			{
				capsLockActive = true;
			}

			var keyValue = (KeyCode)Marshal.ReadInt32(lParam);
			var keyboardLayout = new KeyboardLayout().GetCurrentKeyboardLayout();
			var windowTitle = new Window().CurrentWindowTitle();
			var key = new KeyPressed(keyValue, shiftPressed, capsLockActive, windowTitle, keyboardLayout.ToString());
			if (keyReleasedCallback != null)
			{
				return keyReleasedCallback.Invoke(key);
			}
			return false;

		}

		public void Dispose()
		{
			if (globalKeyboardHookId == IntPtr.Zero)
				User32.UnhookWindowsHookEx(globalKeyboardHookId);
		}
	}
}
