using EasySave2._0.Models.Notifications_Related;
using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave2._0.Models
{
	public class ProcessObserver
	{
		private static ProcessObserver? _ProcessObserverInstance;
		private List<string> DetectedBS = new List<string>();

		public static event EventHandler? BuisnessSoftwareDetected;
		public static event EventHandler? AllBuisnessProcessClosed;

		public bool AnyBSOpened => DetectedBS.Count > 0;

		private ProcessObserver()
		{
			StartProcessObserving();
		}

		public static ProcessObserver GetProcessObserverInstance()
		{
			return _ProcessObserverInstance ??= new ProcessObserver();
		}
		private void StartProcessObserving()
		{
			Task.Run(() =>
			{
				while (true)
				{
					Settings settings = Creator.GetSettingsInstance();
					List<string> runningProcesses = Process.GetProcesses().Select(process => process.ProcessName).ToList();

					var tmp = new List<string>(DetectedBS);

					DetectedBS = settings.BuisnessSoftwaresInterrupt.Intersect(runningProcesses).ToList();
					
					if(DetectedBS.Count > 0 && tmp.Count != DetectedBS.Count)
					{
						NotificationHelper.CreateNotifcation(title: Application.Current.Resources["BuisnessSoftwareTitle"].ToString(),
													 content: Application.Current.Resources["BuisnessSoftwareFound"].ToString(),
													 type: 1);
						BuisnessSoftwareDetected?.Invoke(this, EventArgs.Empty);
					}
					else if(DetectedBS.Count == 0 && tmp.Count != DetectedBS.Count)
					{
						NotificationHelper.CreateNotifcation(title: Application.Current.Resources["BuisnessSoftwareTitle"].ToString(),
													 content: Application.Current.Resources["NoBuisnessSoftwareFound"].ToString(),
													 type: 2);
						AllBuisnessProcessClosed?.Invoke(this, EventArgs.Empty);
					}
				}
			});
		}
	}
}
