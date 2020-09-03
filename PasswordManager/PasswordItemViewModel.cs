using Backend;
using GUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI
{
    public class PasswordItemViewModel : ViewModelBase
    {
        public readonly SettingsModel model;

        public PasswordItemViewModel(SettingsModel model)
        {
            this.model = model;
        }

        public string ApplicationName
        {
            get { return model.ApplicationName; }
            set
            {
                model.ApplicationName = value;
                OnPropertyChanged("ApplicationName");
            }
        }

        public List<string> Password
        {
            get { return model.Password; }
            set
            {
                model.Password = value;
                OnPropertyChanged("Password");
            }
        }

        public string Hotkey
        {
            get { return model.Hotkey; }
            set
            {
                model.Hotkey = value;
                OnPropertyChanged("Hotkey");
            }
        }

        public bool IsHandled
        {
            get { return model.IsHandled; }
            set
            {
                model.IsHandled = value;
                OnPropertyChanged("IsHandled");
            }
        }

    }
}
