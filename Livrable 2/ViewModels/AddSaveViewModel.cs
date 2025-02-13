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

namespace EasySave2._0.ViewModels
{
    public class AddSaveViewModel : ViewModelBase
    {
        public ICommand SaveCommand { get;}

        public Save SaveToAdd { get; } = new Save();

        public AddSaveViewModel()
        {
			SaveToAdd.OnValidateProperty += SaveToAdd_OnValidateProperty;
			SaveCommand = new RelayCommand(_ => CreateSave(), _ => CanCreateSave());
        }

		private string SaveToAdd_OnValidateProperty(string propertyName)
		{
			string error = string.Empty;

			switch (propertyName)
			{
				case nameof(SaveToAdd.Name):
					if (string.IsNullOrWhiteSpace(SaveToAdd.Name))
						error = "Le nom ne peut pas être vide";
					break;

				case nameof(SaveToAdd.SourcePath):
					if (string.IsNullOrWhiteSpace(SaveToAdd.SourcePath))
						error = "Le chemin source ne peut pas être vide. ";
					if (!Directory.Exists(SaveToAdd.SourcePath))
						error += $"Le dossier spécifié n'existe pas !";
					break;

				case nameof(SaveToAdd.DestinationPath):
					if (string.IsNullOrWhiteSpace(SaveToAdd.DestinationPath))
						error = "Le chemin de destination ne peut pas être vide. ";
					if (!Directory.Exists(SaveToAdd.DestinationPath))
						error += $"Le dossier spécifié n'existe pas !";
					break;
			}

			return error;
		}

		private bool CanCreateSave()
        {
            return !string.IsNullOrEmpty(SaveToAdd.Name);
        }

        private void CreateSave()
        {
			saveStore.CreateNewSave(SaveToAdd.Name, SaveType.Full, SaveToAdd.SourcePath, SaveToAdd.DestinationPath);
            ClearFields();
		}

        private void ClearFields()
        {
			SaveToAdd.Name = string.Empty;
			SaveToAdd.SourcePath = string.Empty;
			SaveToAdd.DestinationPath = string.Empty;
		}
    }
}
