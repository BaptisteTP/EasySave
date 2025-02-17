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
        public SettingPage(SettingsViewModel _settingsViewModel)
        {
            InitializeComponent();
            SettingsViewModel viewModel = new SettingsViewModel();
			viewModel.SettingsConfirmed += ViewModel_SettingsConfirmed;
            DataContext = viewModel;
        }
        // ----- Methods -----
        // Select Language in Settings View
        /*public void updateSelectedLanguage()
        {
            if (this.settingsViewModel.model.settings.ActiveLanguage == "en-US")
            {
                EnglishButton.BorderBrush = Brushes.DodgerBlue;
                FrenchButton.BorderBrush = null;
            }
            else if (this.settingsViewModel.model.settings.ActiveLanguage == "fr-FR")
            {
                EnglishButton.BorderBrush = null;
                FrenchButton.BorderBrush = Brushes.DodgerBlue;
            }
        }*/
        private void ViewModel_SettingsConfirmed(object? sender, EventArgs e)
		{
            NavigationService.Navigate(Creator.GetHomePageInstance());
		}
    }
}
