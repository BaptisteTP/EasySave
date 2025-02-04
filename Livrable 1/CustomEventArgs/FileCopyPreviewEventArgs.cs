using Project_Easy_Save.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.CustomEventArgs
{
	public class FileCopyPreviewEventArgs : EventArgs
	{

		public Save ExecutedSave {  get; set; }
		public string State { get; set; }
		public List<string>? EligibleFiles { get; set; }

		public List<string>? RemainingFiles { get; set; }
		public string? CurrentFileSourcePath { get; set; }
		public string? CurrentFileDestinationPath { get; set; }
		public FileCopyPreviewEventArgs(Save executedSave, string state, List<string> eligibleFiles, List<string> remainingFiles, string currentFileSourcePath, string currentFileDestinationPath)
		{
			ExecutedSave = executedSave;
			State = state;
			EligibleFiles = eligibleFiles;
			RemainingFiles = remainingFiles;
			CurrentFileSourcePath = currentFileSourcePath;
			CurrentFileDestinationPath = currentFileDestinationPath;
		}

		public FileCopyPreviewEventArgs(Save executedSave, string state)
		{
			ExecutedSave = executedSave;
			State = state;
		}
	}
}
