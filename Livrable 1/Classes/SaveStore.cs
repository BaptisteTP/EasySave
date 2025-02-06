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
        // This class is responsible for managing the saves.
        // It can create, edit, delete, display and execute saves.

        private List<Save> Saves = [];
        public event EventHandler<Save>? SaveCreated;
        public bool CanAddSave => NumberOfSaves < MaximumNumberOfSave;
        public bool CanExecuteSave => NumberOfSaves > 0;
        public int NumberOfSaves => Saves.Count;
        private int CurrentAvailableID { get; set; } = 1; 
        private int MaximumNumberOfSave { get; } = 5;

        public Save? SaveToEdit { get; set; }

        public Save? SaveToDelete { get; set; }

        public int CreateNewSave(string name, SaveType type, string sourcePath, string destinationPath)
        {
            // Takes information from the user and creates a new save.
            Save newSave = new Save( // Create a new save
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
            // Edits a save's property.
            switch (property)
			{
                // Look for the save with the given id and change the property to the new value.
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
        // Look for the save with the given id and remove it from the list.
        public void DeleteSave(int id) { Saves.Remove(Saves.Find(s => s.Id == id)); }
        // Look for the save with the given id and execute it
        public void ExecuteSave(int id) { Saves.Find(s => s.Id == id).Execute(); }
        // Execute all saves in the list
        public void ExecuteAllSaves() { Saves.ForEach(s => s.Execute()); }
        // Look for the save with the given id and return it
        public Save GetSave(int id) => Saves.Find(s => s.Id == id);
        // Return all saves in the list
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
        // Get the save with the given id and all information in the console
        public void DisplaySave(int id)
        {
            Console.Clear();
            Save save = GetSave(id); // Get the save with the given id
            // Display all information about the save
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

        public void SaveDelete_SelectionChanged(object sender, ConsoleKey e)
        {
            int currentIndexOfSelectedInList = Saves.IndexOf(SaveToDelete!);
            if (e == ConsoleKey.UpArrow) 
            {
                SaveToDelete = Saves[(--currentIndexOfSelectedInList % NumberOfSaves + NumberOfSaves) % NumberOfSaves];
            }
            else if (e == ConsoleKey.DownArrow)
            {
                SaveToDelete = Saves[++currentIndexOfSelectedInList % NumberOfSaves];
            }   
        }
    }
}
