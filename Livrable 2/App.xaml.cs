using EasySave2._0.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EasySave2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		void App_Startup(object sender, StartupEventArgs e)
		{
			MainWindow mainWindow = Creator.GetMainWindow();
			mainWindow.Show();
			mainWindow.StartAppNaviguation();
		}
	}

}
