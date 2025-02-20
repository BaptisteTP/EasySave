using EasySave2._0;
using EasySave2._0.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EasySave2._0.CustomEventArgs;
using System.Windows.Navigation;
using System.Diagnostics;

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
        public ICommand DeleteCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand InformationSaveCommand { get; }

        public ICommand ExecuteAllSavesCommand { get; }
        public event EventHandler<Save> SaveModify;
        public event EventHandler<Save> SaveInfo;

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
                    OnPropertyChanged();
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
                StartSaveCommand = new RelayCommand(StartSave, CanInteract);
                DeleteCommand = new RelayCommand(DeleteItem, CanInteract);
                EditItemCommand = new RelayCommand(EditItem, CanInteract);
                InformationSaveCommand = new RelayCommand(OpenInfoPopup, CanInteract);
                ExecuteAllSavesCommand = new RelayCommand(ExecuteAllSaves);


                UpdatePagedItems();
            }

        private void InfoItem(object obj)
        {
            if (obj is Save save)
            {
                SaveInfo?.Invoke(this, save);
            }
        }

        private void ExecuteAllSaves(object obj)
		    {
			    foreach(Save save in Items)
                {
                    if (!save.IsExecuting)
                    {
				        ExecuteSaveAsync(save);
                    }
                }
		    }

		    private bool CanInteract(object arg)
            {
                if(arg is Save save)
                {
                    return !save.IsExecuting;
                }
                return false;
            }

            private void EditItem(object obj)
            {
                if (obj is Save save)
                {
                    SaveModify?.Invoke(this, save);
                }
            }

		    private async void StartSave(object obj)
		    {
			    if(obj is Save saveToExecute)
			    {
				    await ExecuteSaveAsync(saveToExecute);
			    }
		    }

		    private async Task ExecuteSaveAsync(Save saveToExecute)
		    {
			    bool executionSuccessful = false;
			    try
			    {
				    await saveToExecute.Execute();
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

		    private void UpdatePagedItems()
            {
                PagedItems.Clear();
                if (Items.Count == 0) return;

                var savesToShow = Items.Skip((_currentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
                foreach (var save in savesToShow)
                {
                    PagedItems.Add(save);
                }

			    OnPropertyChanged(nameof(CurrentPageFormatted));
			    OnPropertyChanged(nameof(CurrentPage));
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

        public void UpdateSave()
        {
            var Saves = saveStore.GetAllSaves();
            Items.Clear();
            foreach (var save in Saves)
            {
                Items.Add(save);
            }
            UpdatePagedItems();
		}

        public void DeleteItem(object obj)
        {
            if (obj is Save save)
            {
                Items.Remove(save);
                saveStore.DeleteSave(save.Id);

                CurrentPage = 1;
                UpdatePagedItems();
			}
        }
        private void OpenInfoPopup(object obj)
        {
            if (obj is Save save)
            {
                InfoPopup infoPopup = new InfoPopup
                {
                    SaveToDisplay = save
                };

                Window popupWindow = new Window
                {
                    Content = infoPopup,
                    Width = 800,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    WindowStyle = WindowStyle.ToolWindow,
                    ResizeMode = ResizeMode.NoResize
                };

                popupWindow.ShowDialog();
            }
        }
    }
}
