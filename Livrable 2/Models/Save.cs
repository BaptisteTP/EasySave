
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
	public class Save : ValidationModelBase
    {
		// Create a save attribute for other classes to use.
		public int Id { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; Validate(); OnPropertyChanged(); }
        }

		private string sourcePath;
		public string SourcePath
		{
			get { return sourcePath; }
			set { sourcePath = value; Validate(); OnPropertyChanged(); }
		}

		private string destinationPath;
		public string DestinationPath
		{
			get { return destinationPath; }
			set { destinationPath = value; Validate(); OnPropertyChanged(); }
		}

		public SaveType Type { get; set; }
        public DateTime? LastExecuteDate { get; set; }
        public Save(int id, string name, string sourcePath, string destinationPath, SaveType type)
        {
			this.CanValidate = true;

			Id = id;
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
            LastExecuteDate = null;
        }

        public Save(int id, string name, string sourcePath, string destinationPath, SaveType type, DateTime lastExecutedDate)
        {
			this.CanValidate = true;

			Id = id;
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
            LastExecuteDate = lastExecutedDate;
        }

		public Save()
		{
			this.CanValidate = true;
		}

		//public void Execute()
		//{
		//    // This method is called when the user wants to execute a save.
		//    Creator.GetPasterInstance().BeginCopyPaste(this);
		//}
	}
}
