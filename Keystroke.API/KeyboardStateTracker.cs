using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keystroke.API
{
    public class KeyboardStateTracker
    {
        private readonly KeyboardListener api;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public KeyboardStateTracker()
        {
            this.api = new KeyboardListener();
        }

        public List<KeyPressed> PressedKeys { get; } = new List<KeyPressed>();

        public void StartTracking()
        {
            api.CreateKeyboardHook(OnKeyPressed, OnKeyReleased);
        }

        private bool OnKeyReleased(KeyPressed arg)
        {
            var pressedKey = PressedKeys.FirstOrDefault(key => key.KeyCode != arg.KeyCode);
            if (pressedKey != null)
            {
                PressedKeys.Remove(pressedKey);
                log.Info($"Key Up: {arg.KeyCode}");
            }
            return false;
        }

        private bool OnKeyPressed(KeyPressed arg)
        {
            if (PressedKeys.All(KeyCode => KeyCode.KeyCode != arg.KeyCode))
            {
                PressedKeys.Add(arg);
            }
            return false;
        }
    }
}
