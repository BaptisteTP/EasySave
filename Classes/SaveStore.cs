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
        public object DisplayAllSaves()
        {
            foreach (var save in Saves)
            {
                Console.WriteLine("ID : " + save.Id);
                Console.WriteLine("Nom : " + save.Name);
                Console.WriteLine("Type : " + save.Type);
                Console.WriteLine("Chemin source : " + save.SourcePath);
                Console.WriteLine("Chemin destination : " + save.DestinationPath);
                Console.WriteLine("Dernière date d'exécution : " + save.LastExecuteDate);
            }
            return null;
        }
        public void SetSaveInfo()
        {
            Console.Write("Choisissez un nom pour la sauvegarde : ");
            string Name = Console.ReadLine();
            Console.Write("Choisissez un type pour la sauvegarde : ");
            SaveType Type = (SaveType)Enum.Parse(typeof(SaveType), Console.ReadLine());
            Console.Write("Choisissez un chemin source pour la sauvegarde : ");
            string SourcePath = Console.ReadLine();
            Console.Write("Choisissez un chemin destination pour la sauvegarde : ");
            string DestinationPath = Console.ReadLine();
            CreateNewSave(Name, Type, SourcePath, DestinationPath);
            foreach (var save in Saves)
            {
                Console.WriteLine(save.Id);
                Console.WriteLine(save.Name);
            }
        }
    }
}
