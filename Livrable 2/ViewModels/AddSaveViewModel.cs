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
                    AddError(nameof(SaveName), "Le nom de la sauvegarde ne peut pas être vide.");
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
					AddError(nameof(SourcePath), "Le chemin source ne peut pas être vide.");
				}
                if (!Settings.UserHasRightPermissionInFolder(SourcePath))
                {
					AddError(nameof(SourcePath), "Le chemin source spécifié n'est pas valide.");
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
					AddError(nameof(DestinationPath), "Le chemin destination ne peut pas être vide.");
				}
				if (!Settings.UserHasRightPermissionInFolder(DestinationPath))
				{
					AddError(nameof(DestinationPath), "Le chemin source spécifié n'est pas valide.");
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
