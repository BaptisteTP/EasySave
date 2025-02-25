using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

					DetectedBS = settings.BuisnessSoftwaresInterrupt.Intersect(runningProcesses).ToList();
					
					if(DetectedBS.Count > 0)
					{
						BuisnessSoftwareDetected?.Invoke(this, EventArgs.Empty);
					}
					else
					{
						AllBuisnessProcessClosed?.Invoke(this, EventArgs.Empty);
					}
				}
			});
		}
	}
}
