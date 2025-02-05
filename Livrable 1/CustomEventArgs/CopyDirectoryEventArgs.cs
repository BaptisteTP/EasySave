using Project_Easy_Save.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.CustomEventArgs
{
	public class CopyDirectoryEventArgs : EventArgs
	{

		public Save ExecutedSave { get; set; }
		public string DestinationPath { get; set; }
		public TimeSpan CreationTimeSpan { get; set; }
		public DateTime CopyDate { get; set; }
		public CopyDirectoryEventArgs(DateTime copyDate, Save executedSave, string destinationPath, TimeSpan creationTimeSpan)
		{
			CopyDate = copyDate;
			ExecutedSave = executedSave;
			DestinationPath = destinationPath;
			CreationTimeSpan = creationTimeSpan;
		}
		
		
	}
}
