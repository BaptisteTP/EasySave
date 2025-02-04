using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    public class Save
    {

		public int Id { get; set; }
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
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

		public void Execute()
        {
            Creator.GetPasterInstance().BeginCopyPaste(this);
        }
    }
}
