using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
    public class AddSaveViewModel : ViewModelBase
    {
        public ICommand SaveCommand { get;}

        public Save SaveToAdd { get; set; } = new Save();

        public AddSaveViewModel()
        {
            SaveCommand = new RelayCommand(_ => CreateSave(), _ => CanCreateSave());
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
