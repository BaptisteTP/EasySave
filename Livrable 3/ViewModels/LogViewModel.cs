using EasySave2._0.Enums;
using EasySave2._0.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
	public class LogViewModel : ValidationViewModelBase
	{
		public event EventHandler? NextPageButtonClicked;
		public ICommand ChangePathCommand { get; }

		public ICommand GoNext { get; }

		public string dailyLogPath = Creator.GetSettingsInstance().DailyLogPath!;

		public string DailyLogPath
		{
			get { return dailyLogPath; }
			set
			{
				ClearError(nameof(DailyLogPath));

				dailyLogPath = value;
				OnPropertyChanged();

				if (string.IsNullOrEmpty(DailyLogPath) || !Settings.UserHasRightPermissionInFolder(DailyLogPath))
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

		public LogViewModel()
		{
			ChangePathCommand = new RelayCommand(ChangePath, CanExecute);
			GoNext = new RelayCommand(GoToNextPage, CanGoNext);
		}
		private void ChangePath(object arg)
		{
			string resultFolder;

			var folderDialog = new OpenFolderDialog
			{

			};

			if (folderDialog.ShowDialog() == true)
			{
				resultFolder = folderDialog.FolderName;

				switch (arg.ToString())
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

		private bool CanExecute(object arg)
		{
			return true;
		}

		private void GoToNextPage(object obj)
		{
			Settings.ChangeSetting("DailyLogPath", dailyLogPath);
			Creator.GetSettingsInstance().DailyLogPath = DailyLogPath;

			Settings.ChangeSetting("RealTimeLogPath", RealTimeLogPath);
			Creator.GetSettingsInstance().RealTimeLogPath = RealTimeLogPath;

			Settings.ChangeSetting("LogFormat", selectedLogType.ToString());
			Creator.GetSettingsInstance().LogFormat = selectedLogType.ToString();

			NextPageButtonClicked?.Invoke(this, EventArgs.Empty);
		}

		private bool CanGoNext(object arg)
		{
			return !string.IsNullOrEmpty(DailyLogPath) 
				&& !string.IsNullOrEmpty(RealTimeLogPath)
				&& !HasErrors;

		}
	}
}
