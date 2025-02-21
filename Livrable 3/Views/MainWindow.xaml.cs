using EasySave2._0.Models;
using EasySave2._0.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace EasySave2._0
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		public void StartAppNaviguation()
		{
			Settings _settings = Creator.GetSettingsInstance();

			if (_settings.ActiveLanguage == "")
			{
				MainFrame.Navigate(Creator.GetWelcomePage());
			}
			else if (_settings.DailyLogPath == "" || !Directory.Exists(_settings.DailyLogPath) || !Settings.UserHasRightPermissionInFolder(_settings.DailyLogPath))
			{
				MainFrame.Navigate(Creator.GetLogPageInstance());
			}
			else if (_settings.RealTimeLogPath == "" || !Directory.Exists(_settings.RealTimeLogPath) || !Settings.UserHasRightPermissionInFolder(_settings.RealTimeLogPath))
			{
				MainFrame.Navigate(Creator.GetLogPageInstance());
			}
			else if (_settings.LogFormat == "")
			{
				MainFrame.Navigate(Creator.GetLogPageInstance());
			}
			else
			{
				MainFrame.Navigate(Creator.GetHomePageInstance());
			}
		}
	}
}
