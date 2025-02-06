using Project_Easy_Save.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.CustomEventArgs
{
    // Event arguments for the file copy event
    public class FileCopyEventArgs : EventArgs
	{

		public DateTime CopyDate {  get; set; }
		public Save ExecutedSave { get; set; }
		public FileInfo SourceFile { get; set; }
		public long FileSize => SourceFile.Length;
		public string DestinationPath { get; set; }
		public TimeSpan TransferTime { get; set; }
		public FileCopyEventArgs(DateTime copyDate, Save executedSave, FileInfo sourceFile, string destinationPath, TimeSpan transferTime)
		{
			CopyDate = copyDate;
			ExecutedSave = executedSave;
			SourceFile = sourceFile;
			DestinationPath = destinationPath;
			TransferTime = transferTime;
		}

	}
}
