using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml.Linq;
using System.Windows.Navigation;
using EasySave2._0.Models;
using System.Windows;
using EasySave2._0.Models.Notifications_Related;

namespace EasySave2._0.ViewModels
{
    public class AddSaveViewModel : ValidationViewModelBase
    {
        public event EventHandler? SaveCreated;
        public ICommand SaveCommand { get;}

        private string _saveName;

        public string SaveName
        {
            get { return _saveName; }
            set 
            {
                _saveName = value; 
                OnPropertyChanged();

                ClearError(nameof(SaveName));
                if( _saveName == "")
                {
                    AddError(nameof(SaveName), Application.Current.Resources["SaveNameEmptyMessage"] as string);
                }
            }
        }

        private string _sourcePath;

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                OnPropertyChanged();

				ClearError(nameof(SourcePath));
				if (_sourcePath == "")
				{
					AddError(nameof(SourcePath), Application.Current.Resources["SourcePathEmptyMessage"] as string);
                }
                if (!Settings.UserHasRightPermissionInFolder(SourcePath))
                {
					AddError(nameof(SourcePath), Application.Current.Resources["SourcePathWrongMessage"] as string);
				}

			}
        }

        private string _destinationPath;

        public string DestinationPath
        {
            get { return _destinationPath; }
            set 
            {
                _destinationPath = value; 
                OnPropertyChanged();

				ClearError(nameof(DestinationPath));
				if (_destinationPath == "")
				{
					AddError(nameof(DestinationPath), Application.Current.Resources["DestinationPathEmptyMessage"] as string);
				}
				if (!Settings.UserHasRightPermissionInFolder(DestinationPath))
				{
					AddError(nameof(DestinationPath), Application.Current.Resources["DestinationPathWrongMessage"] as string);
				}
			}
        }
        private SaveType selectedSaveType = SaveType.Full;

        public SaveType SelectedSaveType
        {
            get { return selectedSaveType; }
            set
            {
                selectedSaveType = value;
                OnPropertyChanged();
            }
        }

        private bool _encrypt;
        public bool Encrypt
        {
            get { return _encrypt; }
            set
            {
                _encrypt = value;
                OnPropertyChanged();
            }
        }

        public AddSaveViewModel()
        {
			SaveCommand = new RelayCommand(_ => CreateSave(), _ => CanCreateSave());
        }

		private bool CanCreateSave()
        {
            return !string.IsNullOrEmpty(SaveName)
                && !string.IsNullOrEmpty(SourcePath)
                && !string.IsNullOrEmpty(DestinationPath)
                && HasErrors == false;
        }

        private void CreateSave()
        {
			saveStore.CreateNewSave(SaveName, SelectedSaveType, SourcePath, DestinationPath, Encrypt);

            NotificationHelper.CreateNotifcation(title: Application.Current.Resources["SaveTitle"] as string,
												 content: string.Format(Application.Current.Resources["SaveAdd"] as string, SaveName),
                                                 type:2);
            ClearFields();
            SaveCreated?.Invoke(this, EventArgs.Empty);
		}


		private void ClearFields()
        {
			SaveName = string.Empty;
			ClearError(nameof(SaveName));

			SourcePath = string.Empty;
			ClearError(nameof(SourcePath));

			DestinationPath = string.Empty;
			ClearError(nameof(DestinationPath));

            Encrypt = false;
        }
	}
}
