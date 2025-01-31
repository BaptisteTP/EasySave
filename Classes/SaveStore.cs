using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    internal class SaveStore
    {
        private List<Save> Saves = [];
        public bool CanAddSave { get; set; }
        public int NumberOfSaves { get; set; }
        private int CurrentAvailableID { get; set; } = 1; 
        private int MaximumNumberOfSave { get; } = 5;

        public int CreateNewSave(string name, SaveType type, string sourcePath, string destinationPath)
        {
            Saves.Add(new Save { 
                Id = CurrentAvailableID, 
                Name = name, 
                Type = type, 
                SourcePath = sourcePath, 
                DestinationPath = destinationPath, 
                LastExecuteDate = null
            });
            return CurrentAvailableID++;
        }
        public void EditSave(int ID, string property, object newValue)
        {

        }
        public void DeleteSave(int id) { Saves.Remove(Saves.Find(s => s.Id == id)); }
        public void ExecuteSave(int id) { Saves.Find(s => s.Id == id).Execute(); }
        public void ExecuteAllSaves() { Saves.ForEach(s => s.Execute()); }

        public Save GetSave(int id) => Saves.Find(s => s.Id == id);
        public List<Save> GetAllSaves() => Saves;
    }
}
