using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.Models.Logs_Related
{
	public class FileCopyLog
	{
		public string Name { get; set; }
		public string FileSource { get; set; }
		public string FileTarget { get; set; }
		public double FileSize { get; set; }
		public string FileTransferTime { get; set; }
		public DateTime Time { get; set; }
		public string TimeEncrypt { get; set; }
		public FileCopyLog(string name, string fileSource, string fileTarget, double fileSize, string fileTransferTime, DateTime time, string timeEncrypt)
		{
			Name = name;
			FileSource = fileSource;
			FileTarget = fileTarget;
			FileSize = fileSize;
			FileTransferTime = fileTransferTime;
			TimeEncrypt = timeEncrypt;
			Time = time;
		}

		public FileCopyLog()
		{

		}
	}
}
