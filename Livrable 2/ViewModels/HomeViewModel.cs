using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace EasySave2._0
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private const int ItemsPerPage = 5;
        private int _currentPage = 1;

        public ObservableCollection<ItemViewModel> Items { get; set; }
        public ObservableCollection<ItemViewModel> PagedItems { get; set; }

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public string CurrentPageFormatted
        {
            get => $"{CurrentPage} / {TotalPages}";
        }
        public int TotalPages => (int)Math.Ceiling((double)Items.Count / ItemsPerPage);

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                    OnPropertyChanged(nameof(CurrentPageFormatted));
                    UpdatePagedItems();
                }
            }
        }

        public HomeViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>
            {
                new ItemViewModel("1", "Item 1"),
                new ItemViewModel("2", "Item 2"),
                new ItemViewModel("3", "Item 3"),
                new ItemViewModel("4", "Item 4")
            };

            PagedItems = new ObservableCollection<ItemViewModel>();

            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoNext());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoPrevious());

            UpdatePagedItems();
        }
        private void UpdatePagedItems()
        {
            PagedItems.Clear();
            var itemsToShow = Items.Skip((_currentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
            foreach (var item in itemsToShow)
            {
                PagedItems.Add(item);
            }
        }

        private void NextPage()
        {
            if (CanGoNext())
            {
                CurrentPage++;
            }
        }
        private void PreviousPage()
        {
            if (CanGoPrevious())
            {
                CurrentPage--;
            }
        }
        private bool CanGoNext()
        {
            return _currentPage * ItemsPerPage < Items.Count;
        }
        private bool CanGoPrevious()
        {
            return _currentPage > 1;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
