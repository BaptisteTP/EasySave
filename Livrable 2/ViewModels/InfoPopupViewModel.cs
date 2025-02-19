using EasySave2._0.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave2._0.ViewModels
{
    public class InfoPopupViewModel : INotifyPropertyChanged
    {
        private Save saveToEdit;

        public Save SaveToEdit
        {
            get { return saveToEdit; }
            set
            {
                saveToEdit = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
