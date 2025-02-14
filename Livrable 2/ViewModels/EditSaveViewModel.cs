using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
    public class EditSaveViewModel : ViewModelBase
    {
        public event EventHandler? SaveCreated;
        public ICommand SaveCommand { get; }

        public Save SaveToEdit { get; }
        public EditSaveViewModel(Save save)
        {
            SaveToEdit = save;
            SaveCommand = new RelayCommand(_ => CreateSave(), _ => CanCreateSave());
        }

        private string SaveToAdd_OnValidateProperty(string propertyName)
        {
            string error = string.Empty;

            switch (propertyName)
            {
                case nameof(SaveToEdit.Name):
                    if (string.IsNullOrWhiteSpace(SaveToEdit.Name))
                        error = "Le nom ne peut pas être vide";
                    break;

                case nameof(SaveToEdit.SourcePath):
                    if (string.IsNullOrWhiteSpace(SaveToEdit.SourcePath))
                        error = "Le chemin source ne peut pas être vide. ";
                    if (!Directory.Exists(SaveToEdit.SourcePath))
                        error += $"Le dossier spécifié n'existe pas !";
                    break;

                case nameof(SaveToEdit.DestinationPath):
                    if (string.IsNullOrWhiteSpace(SaveToEdit.DestinationPath))
                        error = "Le chemin de destination ne peut pas être vide. ";
                    if (!Directory.Exists(SaveToEdit.DestinationPath))
                        error += $"Le dossier spécifié n'existe pas !";
                    break;
            }

            return error;
        }

        private bool CanCreateSave()
        {
            return !string.IsNullOrEmpty(SaveToEdit.Name);
        }

        private void CreateSave()
        {
            saveStore.CreateNewSave(SaveToEdit.Name, SaveType.Full, SaveToEdit.SourcePath, SaveToEdit.DestinationPath);
            ClearFields();
            SaveCreated?.Invoke(this, EventArgs.Empty);
        }

        private void ClearFields()
        {
            SaveToEdit.Name = string.Empty;
            SaveToEdit.SourcePath = string.Empty;
            SaveToEdit.DestinationPath = string.Empty;

        }
    }
}
