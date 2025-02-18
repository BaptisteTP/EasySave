using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasySave2._0.ViewModels;
using Microsoft.Win32;
using System.Globalization;

namespace EasySave2._0
{
    /// <summary>
    /// Logique d'interaction pour SettingPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            SettingsViewModel viewModel = new SettingsViewModel();
			viewModel.SettingsConfirmed += ViewModel_SettingsConfirmed;
            DataContext = viewModel;
        }
        public void ApplyLanguageSettings(string LanguageCode)
        {
            // Apply the language settings changes.
            ResourceDictionary dictionary = new ResourceDictionary();
            switch (LanguageCode)
            {
                case "en-US":
                    dictionary.Source = new Uri("Resources/Languages/en-US.xaml", UriKind.Relative);
                    break;
                case "fr-FR":
                    dictionary.Source = new Uri("Resources/Languages/fr-FR.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dictionary);

        }
        private void ViewModel_SettingsConfirmed(object? sender, EventArgs e)
		{
            NavigationService.GoBack();
		}
    }
}
