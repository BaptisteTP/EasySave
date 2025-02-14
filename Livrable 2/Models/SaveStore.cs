using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace EasySave2._0.ViewModels
{
	public class SaveStore
	{
		// This class is responsible for managing the saves.
		// It can create, edit, delete, display and execute saves.

		private List<Save> Saves = [];
		public event EventHandler? SaveCreated;
		public event EventHandler? SaveDeleted;
		public event EventHandler? SaveEdited;
		public bool CanAddSave => NumberOfSaves < MaximumNumberOfSave;
		public bool CanExecuteSave => NumberOfSaves > 0;
		public int NumberOfSaves => Saves.Count;
		private int CurrentAvailableID { get; set; } = 1;
		private int MaximumNumberOfSave { get; } = 5;

		public Save? SaveToEdit { get; set; }

		public Save? SaveToDelete { get; set; }


		public void LoadLoggedSaves()
		{
			Saves.Clear();
			Saves = DeserializeSaves();
			GetAvailableSaveId();
			SaveCreated?.Invoke(this, EventArgs.Empty);
		}

		private void GetAvailableSaveId()
		{
			if(Saves.Count > 0)
			{
				CurrentAvailableID = Saves.OrderBy(save => save.Id).Last().Id + 1;
			}
			else
			{
				CurrentAvailableID = 1;
			}
		}

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
			SaveCreated?.Invoke(this, EventArgs.Empty);

			return CurrentAvailableID++;
		}
		public void EditSave(int id, int property, object newValue)
		{
			// Edits a save's property.
			Save? saveToEdit = Saves.FirstOrDefault(save => save.Id == id);
			if (saveToEdit == null) { return; }

			switch (property)
			{
				// Look for the save with the given id and change the property to the new value.
				case 1:
					saveToEdit.Name = (string)newValue;
					SaveEdited?.Invoke(this, EventArgs.Empty);
					break;

				case 2:
					saveToEdit.SourcePath = (string)newValue;
					SaveEdited?.Invoke(this, EventArgs.Empty);
					break;

				case 3:
					saveToEdit.DestinationPath = (string)newValue;
					SaveEdited?.Invoke(this, EventArgs.Empty);
					break;

				case 4:
					saveToEdit.Type = (SaveType)newValue;
					SaveEdited?.Invoke(this, EventArgs.Empty);
					break;

				default:
					return;
			}
		}
		// Look for the save with the given id and remove it from the list.
		public void DeleteSave(int id)
		{
			Save? saveToDelete = Saves.Find(s => s.Id == id);
			if (saveToDelete == null) { return; }

			Saves.Remove(saveToDelete);
			SaveDeleted?.Invoke(this, EventArgs.Empty);
		}
		// Look for the save with the given id and execute it
		public void ExecuteSave(int id) { Saves.Find(s => s.Id == id).Execute(); }
		// Execute all saves in the list
		public void ExecuteAllSaves() { Saves.ForEach(s => s.Execute()); }

		public async Task ExecuteSavesRange(int start, int stop, Action<int> SaveExecuted, Action<int> OnFailedExecute)
		{
			if (Saves.Count == 0) { throw new InvalidOperationException("Cannot execute specified saves : there are nos saves currently registered."); }
			if (start > stop) { throw new ArgumentException("Start number is greater than stop number."); }

			else
			{
				for (int i = start - 1; i <= stop - 1; i++)
				{
					if (i < Saves.Count)
					{
						SaveExecuted.Invoke(i + 1);
						await Saves.ElementAt(i).Execute();

					}
					else
					{
						OnFailedExecute.Invoke(i + 1);

					}
				}
			}
		}

		public async Task ExecuteSaves(List<int> saveNumbers, Action<int> OnSaveExecute, Action<int> OnFailedExecute)
		{
			if (Saves.Count == 0) { throw new InvalidOperationException("Cannot execute specified saves : there are nos saves currently registered."); }

			foreach (int saveNumber in saveNumbers)
			{
				if (saveNumber > 0 && saveNumber <= Saves.Count)
				{
					OnSaveExecute.Invoke(saveNumber);
					await Saves.ElementAt(saveNumber - 1).Execute();
				}
				else
				{
					OnFailedExecute.Invoke(saveNumber);
				}

			}
		}

		// Look for the save with the given id and return it
		public Save GetSave(int id) => Saves.Find(s => s.Id == id);
		// Return all saves in the list
		public List<Save> GetAllSaves() => Saves;

		public void DisplayAllSaves()
		{
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

		private List<Save> DeserializeSaves()
		{
			string? logFormat = Creator.GetSettingsInstance().LogFormat;
			string pathToSaveslog = "saves/saves." + logFormat;

			if (!File.Exists(pathToSaveslog)) { return new List<Save>(); }

			string logString = File.ReadAllText(pathToSaveslog);

			switch (logFormat)
			{
				case "xml":
					var mySerializer = new XmlSerializer(typeof(List<Save>));
					using (TextReader reader = new StringReader(logString))
					{
						return (List<Save>)mySerializer.Deserialize(reader)!;
					}

				case "json":
					return JsonSerializer.Deserialize<List<Save>>(logString)!;

				default:
					return new List<Save>();
			}
		}
	}
}