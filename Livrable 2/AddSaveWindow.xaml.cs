using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave2._0
{
    /// <summary>
    /// Logique d'interaction pour HomePage.xaml
    /// </summary>
    public partial class AddSaveWindow : Window
    {
        public AddSaveWindow()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }

        private void SourceFolder_click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                // Set options here
            };

            if (folderDialog.ShowDialog() == true)
            {
                SourceFolderBox.Text = folderDialog.FolderName;
                // Do something with the result
            }
        }

        private void DestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                // Set options here
            };

            if (folderDialog.ShowDialog() == true)
            {
                DestinationFolderBox.Text = folderDialog.FolderName;
                // Do something with the result
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomePage HomePage = new HomePage();
            HomePage.Show();
            this.Close();
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow SettingWindow = new SettingWindow();
            SettingWindow.Show();
            this.Close();
        }
    }
}
