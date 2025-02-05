using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class DirectoryCopyLog
	{

		public string Name { get; set; }
		public TimeSpan DirectoryTransferTime { get; set; }
		public DateTime Time { get; set; }
		public DirectoryCopyLog(string name, TimeSpan directoryTransferTime, DateTime time)
		{
			Name = name;
			DirectoryTransferTime = directoryTransferTime;
			Time = time;
		}
	}
}
