using EasySave2._0.Enums;
using EasySave2._0.Models;
using EasySave2._0.Models.Notifications_Related;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
    public class EditSaveViewModel : ValidationViewModelBase
    {
        public event EventHandler? SaveEdit;
        public ICommand ModifCommand { get; }

        private Save saveToEdit;

        public Save SaveToEdit
        {
            get
            {
                return saveToEdit;
            }
            set
            {
                saveToEdit = value;
                SaveName = saveToEdit.Name;
                SourcePath = saveToEdit.SourcePath;
                DestinationPath = saveToEdit.DestinationPath;
                if (Enum.IsDefined(typeof(SaveType), saveToEdit.Type))
                {
                    SelectedSaveType = saveToEdit.Type;
                }
                else
                {
                    SelectedSaveType = SaveType.Full;
                }
                Encrypt = saveToEdit.Encrypt;
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

        private string _saveName;

        public string SaveName
        {
            get { return _saveName; }
            set
            {
                _saveName = value;
                OnPropertyChanged();

                ClearError(nameof(SaveName));
                if (_saveName == "")
                {
                    AddError(nameof(SaveName), "Le nom de la sauvegarde ne peut pas être vide.");
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

        public EditSaveViewModel()
        {
            ModifCommand = new RelayCommand(ModifySave, CanModifySave);
        }

        private bool CanModifySave(object arg)
        {
            return !HasErrors;
        }

        private void ModifySave(object obj)
        {
            if (SaveToEdit != null)
            {
                saveStore.EditSave(SaveToEdit.Id, 1, SaveName);
                saveStore.EditSave(SaveToEdit.Id, 2, SourcePath);
                saveStore.EditSave(SaveToEdit.Id, 3, DestinationPath);
                saveStore.EditSave(SaveToEdit.Id, 4, SelectedSaveType);
                saveStore.EditSave(SaveToEdit.Id, 5, Encrypt);

				NotificationHelper.CreateNotifcation(title: Application.Current.Resources["SaveTitle"] as string,
												 content: string.Format(Application.Current.Resources["SaveEdit"] as string, SaveName),
												 type: 2);

				SaveEdit?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
