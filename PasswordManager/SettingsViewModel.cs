using Backend;
using GUI.Infrastructure;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace GUI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly List<SettingsModel> model;
        private ObservableCollection<PasswordItemViewModel> settings;
        private static readonly log4net.ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public SettingsViewModel(List<SettingsModel> model)
        {
            this.model = model;
            InitializeCommandBindings();
            InitializeSettingsList();
        }

        private void InitializeCommandBindings()
        {
            DeleteRowCommand = new RelayCommand(OnDeleteRowCommandExecute);
        }

        public ObservableCollection<PasswordItemViewModel> Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged("Settings");
            }
        }

        public ICommand DeleteRowCommand { get; set; }

        public void OnDeleteRowCommandExecute(object sender)
        {
            var passwordItem = sender as PasswordItemViewModel;
            // Remove underlying password data
            this.model.Remove(passwordItem.model);
            // Remove row from GUi
            Settings.Remove(passwordItem);
        }

        private void InitializeSettingsList()
        {
            if (model != null)
            {
                var passwordItems = model.Select(m => new PasswordItemViewModel(m)).ToList();
                Settings = new ObservableCollection<PasswordItemViewModel>(passwordItems);
            }
        }

    }
}
