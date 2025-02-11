using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace EasySave2._0
{
    public class ItemViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ItemViewModel(string id, string name)
        {
            ID = id;
            Name = name;
            PlayCommand = new RelayCommand(Play);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
        }

        private void Play(object obj) => Debug.WriteLine($"Play clicked on {Name}");
        private void Edit(object obj) => Debug.WriteLine($"Edit clicked on {Name}");
        private void Delete(object obj) => Debug.WriteLine($"Delete clicked on {Name}");
    }
}
