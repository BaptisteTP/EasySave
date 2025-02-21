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
    /// Logique d'interaction pour WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            var viewModel = new WelcomeViewModel();
            LanguageSelector.LanguageChanged += viewModel.LanguageControl_LanguageChanged;
            viewModel.NextPageButtonClicked += ViewModel_NextPageButtonClicked;
            DataContext = viewModel;
        }

		private void ViewModel_NextPageButtonClicked(object? sender, EventArgs e)
		{
            MainWindow mainWindow = Creator.GetMainWindow();
            NavigationService.Navigate(mainWindow);
			mainWindow.StartAppNaviguation();

		}
	}
}
