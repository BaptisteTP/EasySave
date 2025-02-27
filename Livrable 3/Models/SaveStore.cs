using EasySave2._0.CustomExceptions;
using EasySave2._0.Enums;
using EasySave2._0.Models;
using EasySave2._0.Models.Notifications_Related;
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
		public event EventHandler<Save>? SaveCreated;
		public event EventHandler<Save>? SaveDeleted;
		public event EventHandler<Save>? SaveEdited;
		public event EventHandler<Save>? SavePaused;
		public event EventHandler<Save>? SaveResumed;
		public event EventHandler<Save>? SaveStopped;
		public event EventHandler? SavesLoaded;
		public bool CanAddSave => NumberOfSaves < MaximumNumberOfSave;
		public bool CanExecuteSave => NumberOfSaves > 0;
		public bool AnyBSUp => Creator.GetProcessObserverInstance().AnyBSOpened;
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
			SavesLoaded?.Invoke(this, EventArgs.Empty);
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

		public int CreateNewSave(string name, SaveType type, string sourcePath, string destinationPath, bool encrypt)
		{
			// Takes information from the user and creates a new save.
			Save newSave = new Save( // Create a new save
				CurrentAvailableID,
				name,
				sourcePath,
				destinationPath,
				type,
				encrypt);


			Saves.Add(newSave);
			SaveCreated?.Invoke(this, newSave);

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
					SaveEdited?.Invoke(this, saveToEdit);
					break;

				case 2:
					saveToEdit.SourcePath = (string)newValue;
					SaveEdited?.Invoke(this, saveToEdit);
					break;

				case 3:
					saveToEdit.DestinationPath = (string)newValue;
					SaveEdited?.Invoke(this, saveToEdit);
					break;

				case 4:
					saveToEdit.Type = (SaveType)newValue;
					SaveEdited?.Invoke(this, saveToEdit);
					break;
				case 5:
					saveToEdit.Encrypt = (bool)newValue;
                    SaveEdited?.Invoke(this, saveToEdit);
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
			SaveDeleted?.Invoke(this, saveToDelete);
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
				List<Task> saveToExecute = new List<Task>();
				for (int i = start - 1; i <= stop - 1; i++)
				{
					if (i < Saves.Count)
					{
						SaveExecuted.Invoke(i + 1);
						saveToExecute.Add(Saves.ElementAt(i).Execute());

					}
					else
					{
						OnFailedExecute.Invoke(i + 1);

					}
				}
				try
				{
					await Task.WhenAll(saveToExecute);
				}catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}

		public async Task ExecuteSaves(List<int> saveNumbers, Action<int> OnSaveExecute, Action<int> OnFailedExecute)
		{
			if (Saves.Count == 0) { throw new InvalidOperationException("Cannot execute specified saves : there are nos saves currently registered."); }

			List<Task> saveToExecute = new List<Task>();
			foreach (int saveNumber in saveNumbers)
			{
				if (saveNumber > 0 && saveNumber <= Saves.Count)
				{
					OnSaveExecute.Invoke(saveNumber);
					saveToExecute.Add(Saves.ElementAt(saveNumber - 1).Execute());
				}
				else
				{
					OnFailedExecute.Invoke(saveNumber);
				}
			}
			await Task.WhenAll(saveToExecute);
		}

		public void PauseSave(int id, bool wasSavePausedByUser)
		{
			Save saveToPause = GetSave(id);
			saveToPause.Pause();
			saveToPause.WasSavePausedByUser = wasSavePausedByUser;

			SavePaused?.Invoke(this, GetSave(id));
		}

		public void StopSave(int id)
		{
			Save saveToPause = GetSave(id);
			saveToPause.Stop();

			SaveStopped?.Invoke(this, GetSave(id));
		}
		public void ResumeSave(int id)
		{
			if (AnyBSUp)
			{
				NotificationHelper.CreateNotifcation(title: "Application métier",
													 content: "Impossible de reprendre la sauvegarde, une application métier est lancée.",
													 type: 0);

				throw new BuisnessSoftwareUpException();
			}

			Save saveToResume = GetSave(id);

			if (!saveToResume.IsPaused)
			{
				NotificationHelper.CreateNotifcation(title: "Erreur",
													 content: "Impossible de reprendre la sauvegarde, elle n'est pas en pause.",
													 type: 0);

				throw new SaveNotPausedException();
			}
			else if (Creator.GetPasterInstance().CriticalFilesBeingCopied && saveToResume.IsWaitingForCriticalFiles)
			{
				NotificationHelper.CreateNotifcation(title: "Fichier prioritaire",
													 content: "Impossible de reprendre la sauvegarde, elle attend la copie de fichier prioritaire.",
													 type: 0);

				throw new CriticalFilesCopyException();
			}
			else
			{
				saveToResume.Resume();
				SaveResumed?.Invoke(this, GetSave(id));
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
        public int CountFilesInDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Length;
            }
            else
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }
        }
        public long GetDirectorySize(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                long totalSize = 0;
                string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                }
                return totalSize;
            }
            else
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }
        }

    }
}