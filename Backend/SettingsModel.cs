using System;
using System.Collections.Generic;
using System.Text;

namespace Backend
{
    [Serializable]
    public class SettingsModel
    {
        public string ApplicationName { get; set; }
        public List<string> Password { get; set; }
        public string Hotkey { get; set; }
        public bool IsHandled { get; set; }
    }
}
