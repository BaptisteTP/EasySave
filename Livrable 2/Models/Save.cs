
using EasySave2._0.Enums;
using EasySave2._0.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.ViewModels
{
	public class Save : ModelBase
    {
		// Create a save attribute for other classes to use.
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
			set
			{
				isExecuting = value;
				OnPropertyChanged();
			}
		}

		public SaveType Type { get; set; }
        public DateTime? LastExecuteDate { get; set; }
        public Save(int id, string name, string sourcePath, string destinationPath, SaveType type)
        {
			Id = id;
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
            LastExecuteDate = null;
        }

        public Save(int id, string name, string sourcePath, string destinationPath, SaveType type, DateTime lastExecutedDate)
        {
			Id = id;
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
            LastExecuteDate = lastExecutedDate;
        }

		public Save()
		{

		}

		public async Task Execute(IProgress<int>? progress = null)
		{
			IsExecuting = true;

			await Task.Run(() => Creator.GetPasterInstance().BeginCopyPaste(this, progress));

			IsExecuting = false;
		}
	}
}
