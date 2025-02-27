using EasySave2._0.Models;
using EasySave2._0.Models.Notifications_Related;
using EasySave2._0.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using UserControl_Library;

namespace EasySave2._0
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
			NotificationHelper.NotificationAdded += NotificationHelper_NotificationAdded;
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

		private void NotificationHelper_NotificationAdded(object? sender, Notification_UC e)
		{
			NotificationGrid.Children.Add(e);
			e.StartAnimation();

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = new TimeSpan(0, 0, 4);
			timer.Tick += (sender, args) => DeleteNotification(timer, e);
			timer.Start();
		}

		private async void DeleteNotification(DispatcherTimer timer, Notification_UC e)
		{
			timer.Stop();
			e.EndAnimation();
			await Task.Delay(400);
			NotificationGrid.Children.Remove(e);
		}
	}
}
