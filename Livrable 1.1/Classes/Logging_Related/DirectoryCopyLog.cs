using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    // Class that stores the log of the directory copy
    public class DirectoryCopyLog
	{

		public string Name { get; set; }
		public string DirectoryTransferTime { get; set; }
		public DateTime Time { get; set; }
        // Constructor for the DirectoryCopyLog class
        public DirectoryCopyLog(string name, string directoryTransferTime, DateTime time)
		{
			Name = name;
			DirectoryTransferTime = directoryTransferTime;
			Time = time;
		}

		public DirectoryCopyLog()
		{
			
		}
	}
}
