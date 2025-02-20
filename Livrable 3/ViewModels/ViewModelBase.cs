using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected SaveStore saveStore {  get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public ViewModelBase()
        {
            saveStore = Creator.GetSaveStoreInstance();
        }
    }
}

