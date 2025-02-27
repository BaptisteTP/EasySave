using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.CustomEventArgs
{
	public class FileCopyEventArgs : EventArgs
	{
		public DateTime CopyDate { get; set; }
		public Save ExecutedSave { get; set; }
		public FileInfo SourceFile { get; set; }
		public long FileSize => SourceFile.Length;
		public string DestinationPath { get; set; }
		public TimeSpan? TransferTime { get; set; }
		public TimeSpan? EncryptTime { get; set; }
		public FileCopyEventArgs(DateTime copyDate, Save executedSave, FileInfo sourceFile, string destinationPath, TimeSpan? transferTime, TimeSpan? timeEncrypt)
		{
			CopyDate = copyDate;
			ExecutedSave = executedSave;
			SourceFile = sourceFile;
			DestinationPath = destinationPath;
			TransferTime = transferTime;
			EncryptTime = timeEncrypt; 
		}

	}
}
