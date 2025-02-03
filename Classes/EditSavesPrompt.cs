using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class EditSavesPrompt : PromptBase
	{
		private bool IsInteracting;

		public EditSavesPrompt() : base() { }

		public void Interact()
		{
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
						EditerSauvegarde();
						break;

					case '4':
						DeleteSave();
                        break;

					case '5':
						Quitter();
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
            Console.Write("Choisissez un nom pour la sauvegarde : ");
            string Name = Console.ReadLine();
            Console.Write("Choisissez un type pour la sauvegarde : ");
            SaveType Type = (SaveType)Enum.Parse(typeof(SaveType), Console.ReadLine());
            Console.Write("Choisissez un chemin source pour la sauvegarde : ");
            string SourcePath = Console.ReadLine();
            Console.Write("Choisissez un chemin destination pour la sauvegarde : ");
            string DestinationPath = Console.ReadLine();
            _saveStore.CreateNewSave(Name, Type, SourcePath, DestinationPath);
        }

		private void DisplaySave()
		{
			_saveStore.DisplayAllSaves();
            Console.WriteLine("Choisissez une savegarde à afficher : ");
			_saveStore.DisplaySave(int.Parse(Console.ReadLine()));
        }

		private void EditerSauvegarde()
		{
			Console.Clear();
			Console.WriteLine("Édition d'une sauvegarde");
		}

		private void DeleteSave()
		{
			_saveStore.DisplayAllSaves();
            Console.WriteLine("Choisissez une sauvegarde à supprimer : ");
            _saveStore.DeleteSave(int.Parse(Console.ReadLine()));
        }

		private void Quitter()
		{
			IsInteracting = false;
			Console.Clear();
			Console.WriteLine("Vous avez quitté le menu d'édtion des sauvegardes.");
		}
	}
}
