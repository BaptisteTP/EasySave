using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace EasySave2._0
{
    public partial class SettingWindow : Window
    {
        public SettingWindow() 
        { 
            InitializeComponent();
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                // Set options here
            };

            if (folderDialog.ShowDialog() == true)
            {
                LogFolderPath.Text = folderDialog.FolderName;
                // Do something with the result
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomePage HomePage = new HomePage();
            this.Close();
        }
    }
}
