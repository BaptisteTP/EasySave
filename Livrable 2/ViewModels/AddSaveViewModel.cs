using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
    public class AddSaveViewModel : ViewModelBase
    {
        public ICommand SaveCommand { get;}
        public string Name { get; set; }
        public SaveType type { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }

        public AddSaveViewModel()
        {
            SaveCommand = new RelayCommand(_ => CreateSave(), _ => CanCreateSave());
        }

        private bool CanCreateSave()
        {
            return !string.IsNullOrEmpty(Name);
        }

        private void CreateSave()
        {
            saveStore.CreateNewSave(Name, SaveType.Full, SourcePath, DestinationPath);

        }
    }
}
