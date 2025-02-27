using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var viewModel = new HomeViewModel();
            viewModel.SaveModify += ViewModel_SaveModify;
            DataContext = viewModel;
        }

        private void ViewModel_SaveModify(object? sender, Save e)
        {
            var editPage = new EditSavePage();
            editPage.SaveToEdit = e;
            NavigationService.Navigate(editPage);
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

        private void PasswordButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PasswordPage());
        }
    }
}

