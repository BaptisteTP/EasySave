using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class EditSavesPrompt : PromptBase
	{
        // Class for the interface editing the saves
        private bool IsInteracting;
		public event EventHandler<ConsoleKey>? OnSaveEditSelectionChanged;
		public event EventHandler<ConsoleKey>? OnSaveDeleteSelectionChanged;
		public EditSavesPrompt() : base()
		{
			OnSaveEditSelectionChanged += _saveStore.SaveEdit_SelectionChanged;
            OnSaveDeleteSelectionChanged += _saveStore.SaveDelete_SelectionChanged;

        }

		public void Interact()
		{
            // Method to interact with the user
            Console.Clear();
            IsInteracting = true;

			while (IsInteracting)
			{
                // display the menu to help the user
                DisplayMenu();
				ConsoleKeyInfo choice = Console.ReadKey(true);

                if (choix.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    IsInteracting = false;
                }

                switch (choix.KeyChar)
				{
                    // wait the information from the user
                    case '1':
						DisplaySave();
                        break;

					case '2':
						CreateSave();
                        break;

					case '3':
						HandleSaveEdit();
						break;

					case '4':
						DeleteSave();
                        break;

					case '5':
						Exit();
						break;
				}
			}
		}

		private void DisplayMenu()
		{
            // Display the menu to the user
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("AskForEditActionMessage"))
            Console.WriteLine(string.Format(_resourceManager.GetString($"AskForEditActionMessage"),  _saveStore.NumberOfSaves));
        }

        private void CreateSave()
        {
            // Ask the user for the information to create a new save
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("AskForOperationName"));
            string Name = Console.ReadLine();
            if (Name == "exit")
            {
                return;
            }


            SaveType Type;
            while (true)
            {
                // Ask the user for the type of save between full and differential
                Console.Write(_resourceManager.GetString("AskForOperationType"));
				Console.WriteLine("");
                string typeInput = Console.ReadLine();
                if (typeInput == "exit")
                {
                    return;
                }
                else if (typeInput == "1")
                {
                    Type = SaveType.Full;
                    break;
                }
                else if (typeInput == "2")
                {
                    Type = SaveType.Differential;
                    break;
                }
				else if (typeInput == "exit")
				{
					   return;
				}
                else
                {
					Console.WriteLine(_resourceManager.GetString("WrongOperationType"));
                    Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewSaveType"));
                }
            }

            string SourcePath;
            do
            {
                // Ask the user for the source path and verify if it exists
                Console.Write(_resourceManager.GetString("AskForOperationSourcePath"));
                Console.WriteLine("");
                SourcePath = Console.ReadLine();
                if (SourcePath == "exit")
                {
                    return;
                } 
				else if (!Directory.Exists(SourcePath))
                {
					Console.Clear();
                    Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewPath"));
                }
            } while (!Directory.Exists(SourcePath));

            string DestinationPath;
            do
            {
                // Ask the user for the destination path and verify if it exists
                Console.Write(_resourceManager.GetString("AskForOperationDestinationPath"));
                Console.WriteLine("");
                DestinationPath = Console.ReadLine();
                if (DestinationPath == "exit")
				{
                    return;
                }
                if (!Directory.Exists(DestinationPath) || DestinationPath == SourcePath || !Settings.UserHasRightPermissionInFolder(DestinationPath))
                {
					Console.Clear();
                    Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewPath"));
                }
            } while (!Directory.Exists(DestinationPath) || DestinationPath == SourcePath || !Settings.UserHasRightPermissionInFolder(DestinationPath));

            // Call the method to create a new save
            _saveStore.CreateNewSave(Name, Type, SourcePath, DestinationPath);
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("InformUser_SaveCreate"));
        }

        private void DisplaySave()
		{
            // Display all saves and ask the user to select one
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingAllSaveOperations"));

            if (_saveStore.NumberOfSaves == 0)
            {
                Console.Clear();
                Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
                return;
            }

            _saveStore.DisplayAllSaves();
            Console.WriteLine(_resourceManager.GetString($"MessageBeforeShowingAllSaveOperations"));
            Console.WriteLine(_resourceManager.GetString("InformUser_return"));
            ConsoleKeyInfo choix = Console.ReadKey(true);
            if (choix.Key == ConsoleKey.Escape)
            {
                Console.Clear();
				return;
            }
        }

		private void HandleSaveEdit()
		{
            // Method to edit a save

            if (_saveStore.NumberOfSaves == 0)
            {
                Console.Clear();
                Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
                return;
            }

            Save? saveToEdit = AskUserToSelectSaveToEdit();
			if(saveToEdit == null) { Console.Clear(); return; }

			int propertyToEdit = AskUserWhichPropertyToEdit(saveToEdit);
			if (propertyToEdit == -1) { Console.Clear(); return; }

			object? newValue = AskUserForNewValue(saveToEdit, propertyToEdit);
			if (newValue == null) { Console.Clear(); return; }

			_saveStore.EditSave(saveToEdit.Id, propertyToEdit, newValue);

		}

		private Save? AskUserToSelectSaveToEdit()
		{
            // Ask the user to select a save to edit
            Console.Clear();
			List<Save> saves = _saveStore.GetAllSaves();

			_saveStore.SaveToEdit = saves[0];
			// Display all saves 
			DisplayPossibleSavesToEdit(saves);

			while (true)
			{
				ConsoleKey hitKey = Console.ReadKey(true).Key;

				switch (hitKey)
				{
					case ConsoleKey.UpArrow:
						Console.Clear();
						OnSaveEditSelectionChanged?.Invoke(this, ConsoleKey.UpArrow);
						break;

					case ConsoleKey.DownArrow:
						Console.Clear();
						OnSaveEditSelectionChanged?.Invoke(this, ConsoleKey.DownArrow);
						break;

					case ConsoleKey.Escape:
						Console.Clear();
						return null;

					case ConsoleKey.Enter:
						Console.Clear();
						return _saveStore.SaveToEdit;

					default:
						Console.Clear();
						Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
						break;

				}
				DisplayPossibleSavesToEdit(saves);
			}
		}

		private int AskUserWhichPropertyToEdit(Save selectedSave)
		{
            string? format = _resourceManager.GetString("PrintOperationFormat");
            // Ask the user which property to edit
            while (true)
			{
				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("InformUser_OperationToModifMessage"));
				Console.WriteLine();
				Console.WriteLine(string.Format(format!, selectedSave.Name, selectedSave.SourcePath, selectedSave.DestinationPath, selectedSave.Type.ToString()));
				Console.WriteLine();
				Console.WriteLine(_resourceManager.GetString("AskForUserAction_SaveOperationModification"));
				string? userChoice = Console.ReadLine();
				if (userChoice == "exit") { Console.Clear(); return -1; }
				bool isChoiceValid = int.TryParse(userChoice, out int selectProperty)
					&& selectProperty > 0 && selectProperty < 5;

				while (!isChoiceValid)
				{
					Console.Clear();
					Console.WriteLine(_resourceManager.GetString("InformUser_OperationToModifMessage"));
					Console.WriteLine();
					Console.WriteLine(string.Format(format!, selectedSave.Name, selectedSave.SourcePath, selectedSave.DestinationPath, selectedSave.Type.ToString()));
					Console.WriteLine();
					Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
					Console.WriteLine(_resourceManager.GetString("AskForUserAction_SaveOperationModification"));
					userChoice = Console.ReadLine();
					isChoiceValid = int.TryParse(userChoice, out selectProperty)
					&& selectProperty > 0 && selectProperty < 5;

				}

				return selectProperty;
			}
		}

		private object? AskUserForNewValue(Save selectedSave, int propertyToEdit)
		{
			Console.Clear();
			object newValue;
			switch (propertyToEdit)
			{
				case 1:
					newValue = PromptForChangeOperationName(selectedSave);
					break;
				case 2:
					newValue = PromptForChangeOperationPath("source", selectedSave);
					break;
				case 3:
					newValue = PromptForChangeOperationPath("destination", selectedSave);
					break;
				case 4:
					newValue = PromptForChangeOperationSaveType(selectedSave);
					break;
				default:
					return null;

			}

			return newValue;
		}

		private object PromptForChangeOperationSaveType(Save selectedSave)
		{
            // Inform the user about the operation and ask for a new type of save
            string? format = _resourceManager.GetString("PrintOperationFormat");

			while (true)
			{
				Console.WriteLine(string.Format(format!, selectedSave.Name, selectedSave.SourcePath, selectedSave.DestinationPath, selectedSave.Type.ToString()));

				Console.WriteLine();
				Console.WriteLine(_resourceManager.GetString("AskToUser_NewSaveTypeForOperation"));
				string? newSaveTypeAsString = Console.ReadLine();

				switch (newSaveTypeAsString)
				{
					case "1":
						return SaveType.Full;
					case "2":
						return SaveType.Differential;
					default:
						Console.Clear();
						Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewSaveType"));
						break;
				}
			}
		}

		private object PromptForChangeOperationPath(string type, Save selectedSave)
		{
            // Ask the user for a new path
            string? format = _resourceManager.GetString("PrintOperationFormat");

			while (true)
			{
				Console.WriteLine(string.Format(format!, selectedSave.Name, selectedSave.SourcePath, selectedSave.DestinationPath, selectedSave.Type.ToString()));

				if (type == "source")
				{
					Console.WriteLine(_resourceManager.GetString("AskToUser_NewSourcePathForOperation"));
				}
				else if (type == "destination")
				{
					Console.WriteLine(_resourceManager.GetString("AskToUser_NewDestinationPathForOperation"));
				}
				Console.WriteLine();
				string? newPath = Console.ReadLine();

				if (Directory.Exists(newPath) && newPath != selectedSave.SourcePath && newPath != selectedSave.DestinationPath)
				{
					return newPath;
				}

				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewPath"));
			}
		}

		private object PromptForChangeOperationName(Save selectedSave)
		{
            // Inform the user about the operation and ask for a new name
            string? format = _resourceManager.GetString("PrintOperationFormat");

			while (true)
			{
				Console.WriteLine(string.Format(format!, selectedSave.Name, selectedSave.SourcePath, selectedSave.DestinationPath, selectedSave.Type.ToString()));
				Console.WriteLine();
				Console.WriteLine(_resourceManager.GetString("AskToUser_NewNameForOperation"));
				string? newName = Console.ReadLine();

				if (!string.IsNullOrEmpty(newName))
				{
					return newName;
				}

				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewName"));
			}
		}

        private Save? AskUserToSelectSaveToDelete()
        {
            Console.Clear();
            List<Save> saves = _saveStore.GetAllSaves();
            _saveStore.SaveToDelete = saves[0];
            DisplayPossibleSavesToDelete(saves);
            while (true)
            {
                ConsoleKey hitKey = Console.ReadKey(true).Key;

                switch (hitKey)
                {
                    case ConsoleKey.UpArrow:
                        Console.Clear();
                        OnSaveDeleteSelectionChanged?.Invoke(this, ConsoleKey.UpArrow);
                        break;

                    case ConsoleKey.DownArrow:
                        Console.Clear();
                        OnSaveDeleteSelectionChanged?.Invoke(this, ConsoleKey.DownArrow);
                        break;

                    case ConsoleKey.Escape:
                        Console.Clear();
                        return null;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        return _saveStore.SaveToDelete;

                    default:
                        Console.Clear();
                        Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
                        break;

                }
                DisplayPossibleSavesToDelete(saves);
            }
        }

        private void DisplayPossibleSavesToEdit(List<Save> saves)
		{
            // Display all saves and ask the user to select one with a selector
            int saveIndex = 1;
			foreach (Save save in saves)
			{
				if (save == _saveStore.SaveToEdit)
				{
					Console.WriteLine($"[*] Sauvegarde numéro {saveIndex}");
				}
				else
				{
					Console.WriteLine($"[] Sauvegarde numéro {saveIndex}");
				}

				Console.WriteLine();
				Console.WriteLine($"Nom : {save.Name}");
				Console.WriteLine($"Répertoire source : {save.SourcePath}");
				Console.WriteLine($"Répertoire de destination : {save.DestinationPath}");
				Console.WriteLine($"Type : {save.Type}");
				Console.WriteLine();
				saveIndex++;
			}
            Console.WriteLine(_resourceManager.GetString("InformUser_ChooseSave"));
        }

		private void DeleteSave()
		{

            // Call method to delete a save
			if (_saveStore.NumberOfSaves == 0)
			{
				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
				return;
			}

            Save? SaveToDelete = AskUserToSelectSaveToDelete();
            if (SaveToDelete == null) { Console.Clear(); return; }

			_saveStore.DeleteSave(SaveToDelete.Id);

        }

        private void DisplayPossibleSavesToDelete(List<Save> saves)
        {
            // Display all saves and ask the user to select one with a selector
            int saveIndex = 1;
            foreach (Save save in saves)
            {
                if (save == _saveStore.SaveToDelete)
                {
                    Console.WriteLine($"[*] Sauvegarde numéro {saveIndex}");
                }
                else
                {
                    Console.WriteLine($"[] Sauvegarde numéro {saveIndex}");
                }

                Console.WriteLine();
                Console.WriteLine($"Nom : {save.Name}");
                Console.WriteLine($"Répertoire source : {save.SourcePath}");
                Console.WriteLine($"Répertoire de destination : {save.DestinationPath}");
                Console.WriteLine($"Type : {save.Type}");
                Console.WriteLine();
                saveIndex++;
            }
            Console.WriteLine(_resourceManager.GetString("InformUser_ChooseSave"));
        }

        private void Exit()
		{
            // Exit the interface and return to mainMenuPrompt interface
            IsInteracting = false;
            Console.Clear();
			Console.WriteLine(_resourceManager.GetString("InformUser_SaveOperationFormQuit"));
        }
	}
}
