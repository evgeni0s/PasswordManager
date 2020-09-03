using GUI.Infrastructure;
using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI
{
    public class MainWindowViewModel : ViewModelBase
    {
		private bool isPrimaryPasswordFormVisible;
		private bool isSettingsVisible;
		private bool isNewConfigurationVisible;
		private Backend.PasswordManager passwordManager = new Backend.PasswordManager();
		private SettingsViewModel settingsViewModel;
		private static readonly log4net.ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public MainWindowViewModel()
		{
			InitializeCommandBindings();
			InitializePrimaryPasswordForm();
			passwordManager.SettingsUpdated += PasswordManager_SettingsUpdated;
		}


		public ICommand OpenSettingsCommand { get; set; }
		public ICommand NewConfigurationCommand { get; set; } // remane to DeleteAllSettings
		public ICommand NewPasswordCommand { get; set; } 
		public ICommand SaveSettingsCommand { get; set; } 

		public SettingsViewModel SettingsViewModel
		{
			get => settingsViewModel; 
			set
			{
				settingsViewModel = value;
				OnPropertyChanged("SettingsViewModel");
			}
		}

		public bool IsPrimaryPasswordFormVisible
		{
			get { return isPrimaryPasswordFormVisible; }
			set 
			{ 
				isPrimaryPasswordFormVisible = value;
				OnPropertyChanged("IsPrimaryPasswordFormVisible");
			}
		}


		public bool IsSettingsVisible
		{
			get { return isSettingsVisible; }
			set 
			{ 
				isSettingsVisible = value;
				OnPropertyChanged("IsSettingsVisible");
			}
		}



		public bool IsNewConfigurationVisible
		{
			get { return isNewConfigurationVisible; }
			set 
			{ 
				isNewConfigurationVisible = value;
				OnPropertyChanged("IsNewConfigurationVisible");
			}
		}

		private void PasswordManager_SettingsUpdated()
		{
			InitilizeSettingsViewModel();
		}


		public void OnPasswordKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
			{
				return;
			}

			// User finished Typeing primary password
			var password = (sender as PasswordBox).Password;
			try
			{
				passwordManager.OpenSettings(password);
			}
			catch (BadPasswordException exception)
			{
				log.Error(exception);
				log.Info("Try again or generate new configuration file.");
				IsNewConfigurationVisible = true;
				return;
			}

			InitilizeSettingsViewModel();
			ListenKeyboard();
			IsPrimaryPasswordFormVisible = false;
			IsSettingsVisible = true;
		}

		private void InitializePrimaryPasswordForm()
		{
			IsPrimaryPasswordFormVisible = true;
		}

		private void InitilizeSettingsViewModel()
		{
			if (passwordManager.XmlRootElement.SettingsModels != null)
			{
				SettingsViewModel = new SettingsViewModel(passwordManager.XmlRootElement.SettingsModels);
			}
		}

		private void ListenKeyboard()
		{
			passwordManager.ListenKeyboard();
			passwordManager.TrackKeyboardState();
			log.Info("Password manager is now ready for work.");
			log.Info("Pause/Break					play record");
			log.Info("Pause/Break + Shift			start recording");
			log.Info("Enter							stop recording");
			//Console.WriteLine("Test");
		}

		private void InitializeCommandBindings()
		{
			OpenSettingsCommand = new RelayCommand(OnOpenSettingsExecute);
			NewConfigurationCommand = new RelayCommand(OnNewConfigurationExecute);
			SaveSettingsCommand = new RelayCommand(OnSaveSettingsExecute);
			NewPasswordCommand = new RelayCommand(OnNewPasswordExecute);
		}

		private void OnNewPasswordExecute(object obj)
		{
			passwordManager.NewPassword();
		}

		private void OnSaveSettingsExecute(object obj)
		{
			passwordManager.SaveSettingsInArchive();
		}

		private void OnNewConfigurationExecute(object obj)
		{
			passwordManager.DeleteSettings();
			IsNewConfigurationVisible = false;
		}

		private void OnOpenSettingsExecute(object obj)
		{
			IsSettingsVisible = true;
		}
	}
}
