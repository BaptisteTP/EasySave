using EasySave2._0;
using EasySave2._0.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class HomeViewModel : ViewModelBase
{
    public event EventHandler<Save>? EditSaveRaised;
    private const int ItemsPerPage = 5;
    private int _currentPage = 1;

    public ObservableCollection<ItemViewModel> Items { get; set; }
    public ObservableCollection<ItemViewModel> PagedItems { get; set; }

    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand EditCommand { get; }

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
        Items = new ObservableCollection<ItemViewModel>();
        var Saves = saveStore.GetAllSaves();
        foreach (var save in Saves)
        {
            Items.Add(new ItemViewModel(save.Id.ToString(), save.Name, this));
        }

        PagedItems = new ObservableCollection<ItemViewModel>();

        NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoNext());
        PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoPrevious());
        EditCommand = new RelayCommand(EdtiSave, CanEditSave);

        UpdatePagedItems();
    }

    private bool CanEditSave(object arg)
    {
        return true;
    }

    public void EdtiSave(ItemViewModel item)
    {
        Save save = saveStore.GetSave(int.Parse(item.ID));
        SaveToEdit.Name = save.Name;
    }

    private void UpdatePagedItems()
    {
        PagedItems.Clear();
        if (Items.Count == 0) return;
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

    public void UpdateSave()
    {
        var Saves = saveStore.GetAllSaves();
        Items.Clear();
        foreach (var save in Saves)
        {
            Items.Add(new ItemViewModel(save.Id.ToString(), save.Name, this));
        }
        CurrentPage = 1; // Réinitialiser la page actuelle à 1
        UpdatePagedItems(); // Mettre à jour les éléments paginés
        OnPropertyChanged(nameof(TotalPages)); // Notifier le changement de TotalPages
        OnPropertyChanged(nameof(CurrentPageFormatted)); // Notifier le changement de CurrentPageFormatted
    }

    public void DeleteItem(ItemViewModel item)
    {
        Items.Remove(item);
        UpdatePagedItems();
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CurrentPageFormatted));
        saveStore.DeleteSave(int.Parse(item.ID));
    }

}
