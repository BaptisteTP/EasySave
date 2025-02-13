using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasySave2._0.Models.Logs_Related
{
	public class Dailylog
	{
		public List<SaveLog> Saves { get; set; } = new List<SaveLog>();
	}

	public class SaveLog
	{
		[XmlAttribute("Save")]
		public string Savename { get; set; }
		public List<DirectoryCopyLog> CopiedDirectories { get; set; } = new List<DirectoryCopyLog>();
		public List<FileCopyLog> CopiedFiles { get; set; } = new List<FileCopyLog>();

		public SaveLog(string savename)
		{
			Savename = savename;
		}

		public SaveLog()
		{

		}
	}
}
