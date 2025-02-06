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
			Console.Clear();
            IsInteracting = true;

			while (IsInteracting)
			{
				DisplayMenu();
                ConsoleKeyInfo choix = Console.ReadKey(true);
                Console.Clear();
                if (choix.Key == ConsoleKey.Escape)
                {
                    IsInteracting = false;
                }

                switch (choix.KeyChar)
				{
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
            Console.WriteLine(string.Format(_resourceManager.GetString($"AskForEditActionMessage"),  _saveStore.NumberOfSaves));
        }

        private void CreateSave()
        {
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
                Console.Write(_resourceManager.GetString("AskForOperationDestinationPath"));
                Console.WriteLine("");
                DestinationPath = Console.ReadLine();
                if (DestinationPath == "exit")
				{
                    return;
                }
                if (!Directory.Exists(DestinationPath) || !Settings.UserHasRightPermissionInFolder(DestinationPath))
                {
					Console.Clear();
                    Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewPath"));
                }
            } while (!Directory.Exists(DestinationPath) || !Settings.UserHasRightPermissionInFolder(DestinationPath));

            _saveStore.CreateNewSave(Name, Type, SourcePath, DestinationPath);
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("InformUser_SaveCreate"));
        }

        private void DisplaySave()
		{
            if (_saveStore.NumberOfSaves == 0)
            {
                Console.Clear();
                Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
                return;
            }
			Console.Clear();
            _saveStore.DisplayAllSaves();
            Console.WriteLine(_resourceManager.GetString($"MessageBeforeShowingAllSaveOperations"));
            Console.WriteLine(_resourceManager.GetString("InformUser_return"));
            ConsoleKeyInfo choix = Console.ReadKey(true);
            Console.Clear();
            if (choix.Key == ConsoleKey.Escape)
            {
				return;
            }
        }

		private void HandleSaveEdit()
		{
            if (_saveStore.NumberOfSaves == 0)
            {
                Console.Clear();
                Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
                return;
            }
			Console.Clear();
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
			Console.Clear();
			List<Save> saves = _saveStore.GetAllSaves();
            _saveStore.SaveToEdit = saves[0];
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

				if (Directory.Exists(newPath))
				{
					return newPath;
				}

				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("InformUser_WrongNewPath"));
			}
		}

		private object PromptForChangeOperationName(Save selectedSave)
		{
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
			if (_saveStore.NumberOfSaves == 0)
			{
				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
				return;
			}
			Console.Clear();
            Save? SaveToDelete = AskUserToSelectSaveToDelete();
            if (SaveToDelete == null) { Console.Clear(); return; }

			_saveStore.DeleteSave(SaveToDelete.Id);

        }

        private void DisplayPossibleSavesToDelete(List<Save> saves)
        {
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
            IsInteracting = false;
            Console.Clear();
			Console.WriteLine(_resourceManager.GetString("InformUser_SaveOperationFormQuit"));
        }
	}
}
