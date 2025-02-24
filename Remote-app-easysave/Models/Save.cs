using Remote_app_easysave.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_app_easysave.Models
{
	public class Save : ModelBase
	{
		public int Id { get; set; }

		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; OnPropertyChanged(); }
		}

		private string sourcePath;
		public string SourcePath
		{
			get { return sourcePath; }
			set { sourcePath = value; OnPropertyChanged(); }
		}

		private string destinationPath;
		public string DestinationPath
		{
			get { return destinationPath; }
			set { destinationPath = value; OnPropertyChanged(); }
		}

		private bool isExecuting;
		public bool IsExecuting
		{
			get { return isExecuting; }
			set { isExecuting = value; OnPropertyChanged(); }
		}

		private int progress;
		public int Progress
		{
			get { return progress; }
			set { progress = value; OnPropertyChanged(); }
		}

		private bool encrypt;
		public bool Encrypt
		{
			get { return encrypt; }
			set { encrypt = value; OnPropertyChanged(); }
		}

		private bool isPaused;

		public bool IsPaused
		{
			get { return isPaused; }
			set { isPaused = value; OnPropertyChanged(); }
		}



		public SaveType Type { get; set; }
		public DateTime? LastExecuteDate { get; set; }

		public Save(int id, string name, string sourcePath, string destinationPath, SaveType type, bool encrypt)
		{
			Id = id;
			Name = name;
			SourcePath = sourcePath;
			DestinationPath = destinationPath;
			Type = type;
			Encrypt = encrypt;
			LastExecuteDate = null;
		}

		public Save(int id, string name, string sourcePath, string destinationPath, SaveType type, DateTime lastExecutedDate, bool encrypt)
		{
			Id = id;
			Name = name;
			SourcePath = sourcePath;
			DestinationPath = destinationPath;
			Type = type;
			Encrypt = encrypt;
			LastExecuteDate = lastExecutedDate;
		}

		public Save()
		{
		}
	}
}