using EasySave2._0.Enums;
using EasySave2._0.ViewModels;
using Microsoft.Win32;
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
using Microsoft.Win32;

namespace EasySave2._0
{
    /// <summary>
    /// Logique d'interaction pour AddSavePage.xaml
    /// </summary>
    public partial class AddSavePage : Page
    {
        public AddSavePage()
        {
            InitializeComponent();
            AddSaveViewModel viewModel = new AddSaveViewModel();
            viewModel.SaveCreated += GoBackToHomeView;
            DataContext = viewModel;

        }

        private void SourceFolder_click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                SourceFolderBox.Text = folderDialog.FolderName;
            }
        }

        private void DestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                DestinationFolderBox.Text = folderDialog.FolderName;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Creator.GetHomePageInstance());
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Creator.GetSettingPageInstance());
        }

        private void GoBackToHomeView(object sender, EventArgs e)
        {
            HomePage homePage = Creator.GetHomePageInstance();
            NavigationService.Navigate(homePage);
            ((HomeViewModel)homePage.DataContext).UpdateSave();
        }
    }
}
