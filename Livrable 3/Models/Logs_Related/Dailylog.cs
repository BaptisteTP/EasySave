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
		[XmlAttribute("Id")]
		public int Id { get; set; }

		[XmlAttribute("Save")]
		public string Savename { get; set; }

		[XmlAttribute("State")]
		public string State { get; set; } = "Executed";
		public List<DirectoryCopyLog> CopiedDirectories { get; set; } = new List<DirectoryCopyLog>();
		public List<FileCopyLog> CopiedFiles { get; set; } = new List<FileCopyLog>();

		public SaveLog(int id, string savename)
		{
			Id = id;
			Savename = savename;
		}

		public SaveLog()
		{

		}
	}
}
