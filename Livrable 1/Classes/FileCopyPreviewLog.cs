﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class FileCopyPreviewLog
	{

		public string Name { get; set; }
		public string SourceFilePath { get; set; }
		public string TargetFilePath { get; set; }
		public string State { get; set; }
		public int TotalFileToCopy { get; set; }
		public int TotalFileSize { get; set; }
		public int NbFilesLeftToDo { get; set; }
		public FileCopyPreviewLog(string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, int totalFileSize, int nbFilesLeftToDo)
		{
			Name = name;
			SourceFilePath = sourceFilePath;
			TargetFilePath = targetFilePath;
			State = state;
			TotalFileToCopy = totalFileToCopy;
			TotalFileSize = totalFileSize;
			NbFilesLeftToDo = nbFilesLeftToDo;
		}
	}
}
