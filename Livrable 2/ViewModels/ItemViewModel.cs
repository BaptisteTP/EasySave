using EasySave2._0.ViewModels;
using System.Windows.Input;

public class ItemViewModel
{
    public string ID { get; set; }
    public string Name { get; set; }
    public ICommand PlayCommand { get; set; }
    public ICommand EditCommand { get; set; }
    public ICommand DeleteCommand { get; set; }

    public ItemViewModel(string id, string name, HomeViewModel homeViewModel)
    {
        ID = id;
        Name = name;
        DeleteCommand = new RelayCommand(_ => homeViewModel.DeleteItem(this));
        EditCommand = new RelayCommand(_ => homeViewModel.EdtiSave(this));
    }

    private void Play(object obj) { /* Implementation */ }
    private void Edit(object obj) { /* Implementation */ }
    private void Delete(object obj) { /* Implementation */ }
}
