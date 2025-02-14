using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.Models.Logs_Related
{
	public class FileCopyPreviewLog
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string SourceFilePath { get; set; }
		public string TargetFilePath { get; set; }
		public string State { get; set; }
		public int TotalFileToCopy { get; set; }
		public long TotalFileSize { get; set; }
		public int NbFilesLeftToDo { get; set; }
		public string Progression { get; set; }
		public FileCopyPreviewLog(int id, string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, long totalFileSize, int nbFilesLeftToDo, string progression)
		{
			Id = id;
			Name = name;
			SourceFilePath = sourceFilePath;
			TargetFilePath = targetFilePath;
			State = state;
			TotalFileToCopy = totalFileToCopy;
			TotalFileSize = totalFileSize;
			NbFilesLeftToDo = nbFilesLeftToDo;
			Progression = progression;
		}

		public FileCopyPreviewLog()
		{

		}
	}
}
