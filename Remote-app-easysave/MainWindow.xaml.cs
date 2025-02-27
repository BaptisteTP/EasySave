using Remote_app_easysave.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UserControl_Library;

namespace Remote_app_easysave
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MainWindowViewModel viewModel = new MainWindowViewModel();
			viewModel.NotificationAdded += ViewModel_NotificationAdded;
			DataContext = viewModel;
		}

		private void ViewModel_NotificationAdded(object? sender, Notification_UC e)
		{
			NotificationGrid.Children.Add(e);
			e.StartAnimation();

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = new TimeSpan(0,0,4);
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