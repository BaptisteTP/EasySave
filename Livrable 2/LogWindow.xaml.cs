using System;
using System.Windows;
using Microsoft.Win32;

namespace EasySave2._0
{
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
