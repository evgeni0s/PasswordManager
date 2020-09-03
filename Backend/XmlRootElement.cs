using System;
using System.Collections.Generic;
using System.Text;

namespace Backend
{
    [Serializable]
    public class XmlRootElement
    {
        public List<SettingsModel> SettingsModels { get; set; }
    }
}
