using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.CustomEventArgs
{
	public class CopyDirectoryEventArgs : EventArgs
	{
		public Save ExecutedSave { get; set; }
		public string DestinationPath { get; set; }
		public DateTime CopyDate { get; set; }
		public TimeSpan? CreationTimeSpan { get; set; }
		public CopyDirectoryEventArgs(DateTime copyDate, Save executedSave, string destinationPath, TimeSpan? creationTimeSpan)
		{
			CopyDate = copyDate;
			ExecutedSave = executedSave;
			DestinationPath = destinationPath;
			CreationTimeSpan = creationTimeSpan;
		}


	}
}
