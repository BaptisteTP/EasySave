using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected SaveStore saveStore {  get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ViewModelBase()
        {
            saveStore = Creator.GetSaveStoreInstance();
        }
    }
}

