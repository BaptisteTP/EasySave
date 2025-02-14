using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EasySave2._0.CustomEventArgs;
using EasySave2._0.ViewModels;

namespace EasySave2._0
{
    public class HomeViewModel : ViewModelBase
    {
        private bool _isASaveExecuting = false;

        public bool IsASaveExecuting
		{
            get { return _isASaveExecuting; }
            set { _isASaveExecuting = value; OnPropertyChanged(); }
        }

        private const int ItemsPerPage = 5;
        private int _currentPage = 1;

        public ObservableCollection<Save> Items { get; set; } = new ObservableCollection<Save>();

        public ObservableCollection<Save> PagedItems { get; set; } = new ObservableCollection<Save>();


		public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand StartSaveCommand {  get; }

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

        private int _saveExecutionProgress = 0;
        public int SaveExecutionProgress
        {
            get { return _saveExecutionProgress; }
            set 
            {
                _saveExecutionProgress = value;
                OnPropertyChanged();
            }
        }

        public HomeViewModel()
        {
            var Saves = saveStore.GetAllSaves();
            foreach (var save in Saves)
            {
                Items.Add(save);
            }

            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoNext());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoPrevious());
            StartSaveCommand = new RelayCommand(StartSave, CanStartSave);

            UpdatePagedItems();
        }

		private bool CanStartSave(object arg)
		{
            return !IsASaveExecuting;
		}

		private async void StartSave(object obj)
		{
			if(obj is Save saveToExecute)
            {
				IProgress<int> progress = new Progress<int>(progress =>
				{
                    SaveExecutionProgress = progress;
				});
				bool executionSuccessful = false ;
                try
                {
                    IsASaveExecuting = true;
                    await saveToExecute.Execute(progress);

                    SaveExecutionProgress = 0;
                    executionSuccessful = true;

				}
				catch
                {
                    executionSuccessful = false;

				}
                finally
                {
                    IsASaveExecuting = false;
                }
            }
		}

		private void UpdatePagedItems()
        {
            PagedItems.Clear();
            if (Items.Count == 0) return;
            var savesToShow = Items.Skip((_currentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
            foreach (var save in savesToShow)
            {
                PagedItems.Add(save);
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
        public void UpdateSave()
        {
            var Saves = saveStore.GetAllSaves();
            Items.Clear();
            foreach (var save in Saves)
            {
                Items.Add(save);
            }
            UpdatePagedItems();
            CurrentPage = 1;
        }
	}
}