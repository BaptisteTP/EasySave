
using EasySave2._0.Enums;
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
	public class Save : INotifyPropertyChanged, IDataErrorInfo
    {
		public string Error => string.Empty;
		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

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

		public string this[string columnName]
		{
			get
			{
				string error = string.Empty;

				switch (columnName)
				{
					case nameof(Name):
						if (string.IsNullOrWhiteSpace(Name))
							error = "Le nom ne peut pas être vide";
						break;

					case nameof(SourcePath):
						if (string.IsNullOrWhiteSpace(SourcePath))
							error = "Le chemin source ne peut pas être vide. ";
						if (!Directory.Exists(SourcePath))
							error += $"Le dossier \"{SourcePath}\" n'existe pas !";
						break;

					case nameof(DestinationPath):
						if (string.IsNullOrWhiteSpace(DestinationPath))
							error = "Le chemin de destination ne peut pas être vide. ";
						if (!Directory.Exists(DestinationPath))
							error += $"Le dossier \"{DestinationPath}\" n'existe pas !";
						break;
				}

				return error;
			}
		}

		//public void Execute()
		//{
		//    // This method is called when the user wants to execute a save.
		//    Creator.GetPasterInstance().BeginCopyPaste(this);
		//}
	}
}
