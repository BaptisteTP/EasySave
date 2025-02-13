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
using Microsoft.Win32;

namespace EasySave2._0
{
    /// <summary>
    /// Logique d'interaction pour LogPage.xaml
    /// </summary>
    public partial class LogPage : Page
    {
        public LogPage()
        {
            InitializeComponent();
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Creator.GetHomePageInstance());
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                LogFolderPath.Text = folderDialog.FolderName;
            }
        }
        private void BrowseFolderDay_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                LogFolderPath.Text = folderDialog.FolderName;
            }
        }
    }
}
