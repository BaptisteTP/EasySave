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
		public EditSavesPrompt() : base()
		{
			OnSaveEditSelectionChanged += _saveStore.SaveEdit_SelectionChanged;
		}

		public void Interact()
		{
			Console.Clear();
            IsInteracting = true;

			while (IsInteracting)
			{
				AfficherMenu();
				Console.Write("Votre choix : ");
				ConsoleKeyInfo choix = Console.ReadKey(true);

				switch (choix.KeyChar)
				{
					case '1':
						CreateSave();
						break;

					case '2':
						DisplaySave();
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

		private void AfficherMenu()
		{
			
			Console.WriteLine("==========================================================\n");
			Console.WriteLine("Que voulez-vous faire ?\n");
			Console.WriteLine("1. Créer une sauvegarde");
			Console.WriteLine("2. Afficher une sauvegarde");
			Console.WriteLine("3. Éditer une sauvegarde");
			Console.WriteLine("4. Supprimer une sauvegarde");
			Console.WriteLine("5. Revenir au menu principal\n");
			Console.WriteLine("==========================================================\n");
			Console.WriteLine("Appuyez sur une touche pour continuer\n\n");
		}

		private void CreateSave()
		{
			Console.Clear();
            if (_saveStore.CanAddSave == true)
			{
				Console.Write("Choisissez un nom pour la sauvegarde : ");
				string Name = Console.ReadLine();

				SaveType Type;
				while (true)
				{
					Console.Write("Choisissez un type pour la sauvegarde (1: Full, 2: Differential) : ");
					string typeInput = Console.ReadLine();
					if (typeInput == "1")
					{
						Type = SaveType.Full;
						break;
					}
					else if (typeInput == "2")
					{
						Type = SaveType.Differential;
						break;
					}
					else
					{
						Console.WriteLine("Type de sauvegarde invalide. Veuillez entrer 1 pour Full ou 2 pour Differential.");
					}
				}

				string SourcePath;
				do
				{
					Console.Write("Choisissez un chemin source pour la sauvegarde : ");
					SourcePath = Console.ReadLine();
					if (!Directory.Exists(SourcePath))
					{
						Console.WriteLine("Le chemin source n'existe pas. Veuillez entrer un chemin valide.");
					}
				} while (!Directory.Exists(SourcePath));

				string DestinationPath;
				do
				{
					Console.Write("Choisissez un chemin destination pour la sauvegarde : ");
					DestinationPath = Console.ReadLine();
					if (!Directory.Exists(DestinationPath))
					{
						Console.WriteLine("Le chemin destination n'existe pas. Veuillez entrer un chemin valide.");
					}
				} while (!Directory.Exists(DestinationPath));

				_saveStore.CreateNewSave(Name, Type, SourcePath, DestinationPath);
				Console.Clear();
				Console.WriteLine("La sauvegarde a été créée.");
			}
			else
			{
				Console.WriteLine("Vous avez atteint le nombre maximum de sauvegardes.");
			}
		}



        private void DisplaySave()
		{
			_saveStore.DisplayAllSaves();
            Console.WriteLine("Choisissez une savegarde à afficher : ");
			_saveStore.DisplaySave(int.Parse(Console.ReadLine()));
        }

		private void HandleSaveEdit()
		{
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
		}

		private void DeleteSave()
		{
			_saveStore.DisplayAllSaves();
            Console.WriteLine("Choisissez une sauvegarde à supprimer : ");
            _saveStore.DeleteSave(int.Parse(Console.ReadLine()));
			Console.Clear();
            Console.WriteLine("La sauvegarde a été supprimée.");
        }

		private void Exit()
		{
			IsInteracting = false;
			Console.Clear();
			Console.WriteLine("Vous avez quitté le menu d'édtion des sauvegardes.");
		}
	}
}
