using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySave2._0
{
    /// <summary>
    /// Logique d'interaction pour HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            var viewmodel = new HomeViewModel();
            viewmodel.EditSaveRaised += Viewmodel_EditSaveRaised;
            DataContext = new HomeViewModel();
        }

        private void Viewmodel_EditSaveRaised(object? sender, Save save)
        {
            NavigationService.Navigate(new EditSavePage(save));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Creator.GetAddSavePageInstance());
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingPage());
        }
    }
}

