using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    public class SaveStore
    {
        private List<Save> Saves = [];
        public event EventHandler<Save>? SaveCreated;
        public bool CanAddSave => NumberOfSaves < MaximumNumberOfSave;
        public int NumberOfSaves => Saves.Count;
        private int CurrentAvailableID { get; set; } = 1; 
        private int MaximumNumberOfSave { get; } = 5;

        public Save? SaveToEdit { get; set; }

        public int CreateNewSave(string name, SaveType type, string sourcePath, string destinationPath)
        {
            Save newSave = new Save(
                CurrentAvailableID,
                name,
                sourcePath,
                destinationPath,
                type);

			Saves.Add(newSave);
            SaveCreated?.Invoke(this, newSave);

            return CurrentAvailableID++;
        }
        public void EditSave(int id, int property, object newValue)
        {
			switch (property)
			{
				case 1:
					Saves.FirstOrDefault(save => save.Id == id)!.Name = (string)newValue;
					break;
				case 2:
					Saves.FirstOrDefault(save => save.Id == id)!.SourcePath = (string)newValue;
					break;
				case 3:
					Saves.FirstOrDefault(save => save.Id == id)!.DestinationPath = (string)newValue;
					break;
				case 4:
					Saves.FirstOrDefault(save => save.Id == id)!.Type = (SaveType)newValue;
					break;
				default:
					return;
			}
		}
        public void DeleteSave(int id) { Saves.Remove(Saves.Find(s => s.Id == id)); }
        public void ExecuteSave(int id) { Saves.Find(s => s.Id == id).Execute(); }
        public void ExecuteAllSaves() { Saves.ForEach(s => s.Execute()); }

        public Save GetSave(int id) => Saves.Find(s => s.Id == id);
        public List<Save> GetAllSaves() => Saves;
        public void DisplayAllSaves()
        {
            Console.Clear();
            foreach (var save in Saves)
            {
                Console.WriteLine("===== Save Number : " + save.Id + " =====");
                Console.WriteLine("Nom : " + save.Name);
                Console.WriteLine("Type : " + save.Type);
                Console.WriteLine("Chemin source : " + save.SourcePath);
                Console.WriteLine("Chemin destination : " + save.DestinationPath);
                Console.WriteLine("Dernière date d'exécution : " + save.LastExecuteDate + "\n");
            }
        }
        public void DisplaySave(int id)
        {
            Console.Clear();
            Save save = GetSave(id);
            Console.WriteLine("ID : " + save.Id);
            Console.WriteLine("Nom : " + save.Name);
            Console.WriteLine("Type : " + save.Type);
            Console.WriteLine("Chemin source : " + save.SourcePath);
            Console.WriteLine("Chemin destination : " + save.DestinationPath);
            Console.WriteLine("Dernière date d'exécution : " + save.LastExecuteDate + "\n");
        }

        public void SaveEdit_SelectionChanged(object sender, ConsoleKey e)
        {
			int currentIndexOfSelectedInList = Saves.IndexOf(SaveToEdit!);
			if (e == ConsoleKey.UpArrow)
			{
				SaveToEdit = Saves[(--currentIndexOfSelectedInList % NumberOfSaves + NumberOfSaves) % NumberOfSaves];
			}
			else if (e == ConsoleKey.DownArrow)
			{
				SaveToEdit = Saves[++currentIndexOfSelectedInList % NumberOfSaves];
			}
		}
    }
}
