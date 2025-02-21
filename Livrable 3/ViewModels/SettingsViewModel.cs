using EasySave2._0.Enums;
using EasySave2._0.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EasySave2._0.ViewModels
{
	public class SettingsViewModel : ValidationViewModelBase
	{
		public event EventHandler? SettingsConfirmed;
		public ICommand ChangedLogPathCommand { get; }
		public ICommand ConfirmSettingsCommand { get; }
		public ICommand AddBuisnessSoftwareCommand { get; }
		public ICommand DeleteBuisnessSoftwareCommand { get; }

		public ObservableCollection<LanguageItem> LanguageItems { get; } = new ObservableCollection<LanguageItem>(Creator.GetAvalaibleLanguages());

		#region Attributes

		public ObservableCollection<string> BuisnessSoftwaresInterrupt { get; }

		private string _buisnessSoftwareToAdd = "";

		public string BuisnessSoftwareToAdd
		{
			get { return _buisnessSoftwareToAdd; }
			set
			{
				_buisnessSoftwareToAdd = value;
				OnPropertyChanged();
			}
		}

		public LanguageItem SelectedLanguage { get; private set; }

		public string dailyLogPath = Creator.GetSettingsInstance().DailyLogPath!;

		public string DailyLogPath
		{
			get { return dailyLogPath; }
			set
			{
				ClearError(nameof(DailyLogPath));

				dailyLogPath = value;
				OnPropertyChanged();

				if(string.IsNullOrEmpty(DailyLogPath) || !Settings.UserHasRightPermissionInFolder(DailyLogPath))
				{
					AddError(nameof(DailyLogPath), Application.Current.Resources["DailyLogUnvalidMessage"] as string);
				}
			}
		}

		public string realTimeLogPath = Creator.GetSettingsInstance().RealTimeLogPath!;
		public string RealTimeLogPath
		{
			get { return realTimeLogPath; }
			set
			{
				ClearError(nameof(RealTimeLogPath));

				realTimeLogPath = value;
				OnPropertyChanged();

                if (string.IsNullOrEmpty(RealTimeLogPath) || !Settings.UserHasRightPermissionInFolder(RealTimeLogPath))
				{
					AddError(nameof(RealTimeLogPath), Application.Current.Resources["RealTimeLogUnvalidMessage"] as string);
				}
			}
		}

		private LogType selectedLogType = LogType.json;

		public LogType SelectedLogType
		{
			get { return selectedLogType; }
			set
			{
				selectedLogType = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public SettingsViewModel()
		{
			ChangedLogPathCommand = new RelayCommand(ChangeViaFolderDialog);
			ConfirmSettingsCommand = new RelayCommand(ConfirmSettings, CanConfirmSettings);
			AddBuisnessSoftwareCommand = new RelayCommand(AddBuisnessSoftware, CanAddBuisness);
			DeleteBuisnessSoftwareCommand = new RelayCommand(DeleteBuisnessSoftware, _ => true);

			Settings settings = Creator.GetSettingsInstance();

			SelectedLanguage = Creator.GetAvalaibleLanguages().FirstOrDefault(lang => lang.Language == settings.ActiveLanguage)
								?? Creator.GetAvalaibleLanguages().FirstOrDefault(lang => lang.Language == settings.FallBackLanguage);
			DailyLogPath = settings.DailyLogPath;
			RealTimeLogPath = settings.RealTimeLogPath;
			BuisnessSoftwaresInterrupt = new ObservableCollection<string>(settings.BuisnessSoftwaresInterrupt);

			if (Enum.IsDefined(typeof(LogType), settings.LogFormat))
			{
				SelectedLogType = (LogType)Enum.Parse(typeof(LogType), settings.LogFormat);
			}
			else
			{
				SelectedLogType = LogType.json;
			}
		}

		private void DeleteBuisnessSoftware(object obj)
		{
			if(obj is string softwareToRemove)
			{
				Settings settings = Creator.GetSettingsInstance();

				BuisnessSoftwaresInterrupt.Remove(softwareToRemove);

				Settings.ChangeSetting("BuisnessSoftwaresInterrupt", new List<string>(BuisnessSoftwaresInterrupt));
				settings.BuisnessSoftwaresInterrupt = new List<string>(BuisnessSoftwaresInterrupt);
			}
		}

		private void AddBuisnessSoftware(object obj)
		{
			Settings settings = Creator.GetSettingsInstance();

			BuisnessSoftwaresInterrupt.Add(BuisnessSoftwareToAdd);
			BuisnessSoftwareToAdd = "";

			Settings.ChangeSetting("BuisnessSoftwaresInterrupt", new List<string>(BuisnessSoftwaresInterrupt));
			settings.BuisnessSoftwaresInterrupt = new List<string>(BuisnessSoftwaresInterrupt);
		}

		private bool CanAddBuisness(object arg)
		{
			return BuisnessSoftwareToAdd != "";
		}

        private void ConfirmSettings(object obj)
		{
			Settings.ChangeSetting("DailyLogPath", dailyLogPath);
			Creator.GetSettingsInstance().DailyLogPath = DailyLogPath;

			Settings.ChangeSetting("RealTimeLogPath", RealTimeLogPath);
			Creator.GetSettingsInstance().RealTimeLogPath = RealTimeLogPath;

			Settings.ChangeSetting("LogFormat", selectedLogType.ToString());
			Creator.GetSettingsInstance().LogFormat = selectedLogType.ToString();

			SettingsConfirmed?.Invoke(this, EventArgs.Empty);
		}

		private bool CanConfirmSettings(object arg)
		{
			return !string.IsNullOrEmpty(dailyLogPath)
				&& !string.IsNullOrEmpty(realTimeLogPath)
				&& !HasErrors;
		}

		private void ChangeViaFolderDialog(object obj)
		{
			string resultFolder;

			var folderDialog = new OpenFolderDialog
			{

			};

			if (folderDialog.ShowDialog() == true)
			{
				resultFolder = folderDialog.FolderName;

				switch (obj.ToString())
				{
					case "daily":

						DailyLogPath = resultFolder;
						break;

					case "realtime":

						RealTimeLogPath = resultFolder;
						break;
				}

			}
		}

		public void LanguageControl_LanguageChanged(object? sender, LanguageItem e)
		{
			Settings.ChangeSetting("ActiveLanguage", e.Language);
			Settings.ApplyLanguageSettings();
		}
	}
}
