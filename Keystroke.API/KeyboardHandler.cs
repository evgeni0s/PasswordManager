using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace Keystroke.API
{
    public class KeyboardHandler
    {
        private InputSimulator inputSimulator;
        private int KeyStrokeDelayMs = 50;
        private readonly ILog log;
        private readonly KeyboardStateTracker stateTracker;

        public KeyboardHandler(KeyboardStateTracker stateTracker)
        {
            try
            {
                inputSimulator = new InputSimulator();
                this.stateTracker = stateTracker;
                this.log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch (Exception e)
            {
                int i = 0;
                i++;
            }
        }

        public bool SendKey(string key)
        {
            this.inputSimulator = new InputSimulator();
            if (key == string.Empty)
            {
                inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SPACE)
                    .KeyUp(VirtualKeyCode.SPACE);
            }
            else if (key[0] == '<')
            {
                var vk = StringToVirtualKey(key);
                if (vk == null)
                {
                    return false;
                }
                // Run special command like CTRL or ENTER or TAB
                inputSimulator.Keyboard.KeyPress(vk.Value);
            }
            else
            {
                inputSimulator.Keyboard.TextEntry(key).Sleep(KeyStrokeDelayMs);
            }
            return true;
        }

        public bool IsManualKeyCombinationPressed(string combination)
        {
            var combinationKeys = GetManualKeyCombination(combination);
            if (combinationKeys == null)
            {
                return false;
            }
            var pressedKeys = stateTracker.PressedKeys.Select(el => Convert(el.KeyCode)).ToList();
            var allOfList1IsInList2 = combinationKeys.Intersect(pressedKeys).Count() == combinationKeys.Count();
            return allOfList1IsInList2;
        }

        // Input Ctrl+Alt+Q
        private static IEnumerable<VirtualKeyCode?> GetManualKeyCombination(string hotkeyCombination)
        {
            if (string.IsNullOrEmpty(hotkeyCombination) || hotkeyCombination == "Auto")
            {
                return null;
            }
            var combinationSplit = hotkeyCombination.Split('+');
            var convertedValues = combinationSplit.Select(StringToVirtualKey);
            if (convertedValues.All(v => v.HasValue))
            {
                return convertedValues.ToList();
            }
            return null;
        }

        public static VirtualKeyCode? Convert(KeyCode keyCode)
        {
            if (VirtualKeyCodesMapping.ContainsValue(keyCode))
            {
                // Main mapping
                return VirtualKeyCodesMapping.FirstOrDefault(kvp => kvp.Value == keyCode).Key;
            }
            // Left-Right fixations
            return AdditionalVirtualKeyCodeMapping(keyCode);
        }

        public static KeyCode? ConvertBack(VirtualKeyCode keyCode)
        {
            if (VirtualKeyCodesMapping.ContainsKey(keyCode))
            {
                return VirtualKeyCodesMapping[keyCode];
            }
            return null;
        }

        public static VirtualKeyCode? AdditionalVirtualKeyCodeMapping(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.LControlKey:
                case KeyCode.RControlKey:
                    return VirtualKeyCode.CONTROL;
                case KeyCode.LShiftKey:
                case KeyCode.RShiftKey:
                    return VirtualKeyCode.SHIFT;
                case KeyCode.LMenu:
                case KeyCode.RMenu:
                    return VirtualKeyCode.MENU;
                case KeyCode.LWin:
                case KeyCode.RWin:
                    return VirtualKeyCode.LWIN;
                default:
                    return (VirtualKeyCode?)null;
            }
        }

        public static Dictionary<VirtualKeyCode, KeyCode> VirtualKeyCodesMapping = new Dictionary<VirtualKeyCode, KeyCode>()
        {
            { VirtualKeyCode.F1, KeyCode.F1},
            { VirtualKeyCode.F2, KeyCode.F2},
            { VirtualKeyCode.F3, KeyCode.F3},
            { VirtualKeyCode.F4, KeyCode.F4},
            { VirtualKeyCode.F5, KeyCode.F5},
            { VirtualKeyCode.F6, KeyCode.F6},
            { VirtualKeyCode.F7, KeyCode.F7},
            { VirtualKeyCode.F8, KeyCode.F8},
            { VirtualKeyCode.F9, KeyCode.F9},
            { VirtualKeyCode.F10, KeyCode.F10},
            { VirtualKeyCode.F11, KeyCode.F11},
            { VirtualKeyCode.F12, KeyCode.F12},

            { VirtualKeyCode.PRINT, KeyCode.PrintScreen},
            { VirtualKeyCode.SCROLL, KeyCode.Scroll},
            { VirtualKeyCode.PAUSE, KeyCode.Pause},
            { VirtualKeyCode.INSERT, KeyCode.Insert},
            { VirtualKeyCode.HOME, KeyCode.Home},
            { VirtualKeyCode.DELETE, KeyCode.Delete},
            { VirtualKeyCode.END, KeyCode.End},
            { VirtualKeyCode.ESCAPE, KeyCode.Escape},
            { VirtualKeyCode.NUMLOCK, KeyCode.NumLock},
            { VirtualKeyCode.TAB, KeyCode.Tab},
            { VirtualKeyCode.BACK, KeyCode.Back},
            { VirtualKeyCode.RETURN, KeyCode.Return},
            { VirtualKeyCode.LEFT, KeyCode.Left},
            { VirtualKeyCode.UP, KeyCode.Up},
            { VirtualKeyCode.DOWN, KeyCode.Down},

            { VirtualKeyCode.MENU, KeyCode.Menu},
            { VirtualKeyCode.LWIN, KeyCode.LWin},
            { VirtualKeyCode.CAPITAL, KeyCode.Capital},
            { VirtualKeyCode.CONTROL, KeyCode.Control},
            { VirtualKeyCode.SHIFT, KeyCode.Shift},
            { VirtualKeyCode.VOLUME_DOWN, KeyCode.VolumeDown},
            { VirtualKeyCode.VOLUME_MUTE, KeyCode.VolumeMute},
            { VirtualKeyCode.VOLUME_UP, KeyCode.VolumeUp},
            { VirtualKeyCode.OEM_PLUS, KeyCode.Oemplus},
            { VirtualKeyCode.VK_0, KeyCode.D0},
            { VirtualKeyCode.VK_1, KeyCode.D1},
            { VirtualKeyCode.VK_2, KeyCode.D2},
            { VirtualKeyCode.VK_3, KeyCode.D3},
            { VirtualKeyCode.VK_4, KeyCode.D4},
            { VirtualKeyCode.VK_5, KeyCode.D5},
            { VirtualKeyCode.VK_6, KeyCode.D6},
            { VirtualKeyCode.VK_7, KeyCode.D7},
            { VirtualKeyCode.VK_8, KeyCode.D8},
            { VirtualKeyCode.VK_9, KeyCode.D9},

            { VirtualKeyCode.VK_Q, KeyCode.Q},
            { VirtualKeyCode.VK_W, KeyCode.W},
            { VirtualKeyCode.VK_E, KeyCode.E},
            { VirtualKeyCode.VK_R, KeyCode.R},
            { VirtualKeyCode.VK_T, KeyCode.T},
            { VirtualKeyCode.VK_U, KeyCode.U},
            { VirtualKeyCode.VK_I, KeyCode.I},
            { VirtualKeyCode.VK_O, KeyCode.O},
            { VirtualKeyCode.VK_P, KeyCode.P},
            { VirtualKeyCode.VK_A, KeyCode.A},
            { VirtualKeyCode.VK_S, KeyCode.S},
            { VirtualKeyCode.VK_D, KeyCode.D},
            { VirtualKeyCode.VK_F, KeyCode.F},
            { VirtualKeyCode.VK_G, KeyCode.G},
            { VirtualKeyCode.VK_H, KeyCode.H},
            { VirtualKeyCode.VK_J, KeyCode.J},
            { VirtualKeyCode.VK_K, KeyCode.K},
            { VirtualKeyCode.VK_L, KeyCode.L},
            { VirtualKeyCode.VK_Z, KeyCode.Z},
            { VirtualKeyCode.VK_X, KeyCode.X},
            { VirtualKeyCode.VK_C, KeyCode.C},
            { VirtualKeyCode.VK_V, KeyCode.V},
            { VirtualKeyCode.VK_B, KeyCode.B},
            { VirtualKeyCode.VK_N, KeyCode.N},
            { VirtualKeyCode.VK_M, KeyCode.M},
        };

        public static VirtualKeyCode? StringToVirtualKey(string keyStr)
        {
            switch (keyStr)
            {
                case "<F1>": return VirtualKeyCode.F1;
                case "<F2>": return VirtualKeyCode.F2;
                case "<F3>": return VirtualKeyCode.F3;
                case "<F4>": return VirtualKeyCode.F4;
                case "<F5>": return VirtualKeyCode.F5;
                case "<F6>": return VirtualKeyCode.F6;
                case "<F7>": return VirtualKeyCode.F7;
                case "<F8>": return VirtualKeyCode.F8;
                case "<F9>": return VirtualKeyCode.F9;
                case "<F10>": return VirtualKeyCode.F10;
                case "<F11>": return VirtualKeyCode.F11;
                case "<F12>": return VirtualKeyCode.F12;
                case "<print screen>": return VirtualKeyCode.PRINT;
                case "<scroll>": return VirtualKeyCode.SCROLL;
                case "<pause>": return VirtualKeyCode.PAUSE;
                case "<insert>": return VirtualKeyCode.INSERT;
                case "<home>": return VirtualKeyCode.HOME;
                case "<delete>": return VirtualKeyCode.DELETE;
                case "<end>": return VirtualKeyCode.END;
                case "<esc>": return VirtualKeyCode.ESCAPE;
                case "<numlock>": return VirtualKeyCode.NUMLOCK;
                case "<tab>": return VirtualKeyCode.TAB;
                case "<backspace>": return VirtualKeyCode.BACK;
                case "<enter>": return VirtualKeyCode.RETURN;
                case " ": return VirtualKeyCode.SPACE;
                case "<left>": return VirtualKeyCode.LEFT;
                case "<up>": return VirtualKeyCode.UP;
                case "<right>": return VirtualKeyCode.RIGHT;
                case "<down>": return VirtualKeyCode.DOWN;
                case "<alt>": return VirtualKeyCode.MENU;
                case "<win>": return VirtualKeyCode.LWIN;
                case "<capsLock>": return VirtualKeyCode.CAPITAL;
                case "<ctrl>": return VirtualKeyCode.CONTROL;
                case "<shift>": return VirtualKeyCode.SHIFT;
                case "<volumeDown>": return VirtualKeyCode.VOLUME_DOWN;
                case "<volumeUp>": return VirtualKeyCode.VOLUME_UP;
                case "<volumeMute>": return VirtualKeyCode.VOLUME_MUTE;
                case "+": return VirtualKeyCode.OEM_PLUS;
                case "0": return VirtualKeyCode.VK_0;
                case "1": return VirtualKeyCode.VK_1;
                case "2": return VirtualKeyCode.VK_2;
                case "3": return VirtualKeyCode.VK_3;
                case "4": return VirtualKeyCode.VK_4;
                case "5": return VirtualKeyCode.VK_5;
                case "6": return VirtualKeyCode.VK_6;
                case "7": return VirtualKeyCode.VK_7;
                case "8": return VirtualKeyCode.VK_8;
                case "9": return VirtualKeyCode.VK_9;
                case "q": return VirtualKeyCode.VK_Q;
                case "w": return VirtualKeyCode.VK_W;
                case "e": return VirtualKeyCode.VK_E;
                case "r": return VirtualKeyCode.VK_R;
                case "t": return VirtualKeyCode.VK_T;
                case "y": return VirtualKeyCode.VK_Y;
                case "u": return VirtualKeyCode.VK_U;
                case "i": return VirtualKeyCode.VK_I;
                case "o": return VirtualKeyCode.VK_O;
                case "p": return VirtualKeyCode.VK_P;
                case "a": return VirtualKeyCode.VK_A;
                case "s": return VirtualKeyCode.VK_S;
                case "d": return VirtualKeyCode.VK_D;
                case "f": return VirtualKeyCode.VK_F;
                case "g": return VirtualKeyCode.VK_G;
                case "h": return VirtualKeyCode.VK_H;
                case "j": return VirtualKeyCode.VK_J;
                case "k": return VirtualKeyCode.VK_K;
                case "l": return VirtualKeyCode.VK_L;
                case "z": return VirtualKeyCode.VK_Z;
                case "x": return VirtualKeyCode.VK_X;
                case "c": return VirtualKeyCode.VK_C;
                case "v": return VirtualKeyCode.VK_V;
                case "b": return VirtualKeyCode.VK_B;
                case "n": return VirtualKeyCode.VK_N;
                case "m": return VirtualKeyCode.VK_M;
                default:
                    return default(VirtualKeyCode);

            }
        }
    }
}
