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



        private void DisplaySave()
		{
			_saveStore.DisplayAllSaves();
            Console.WriteLine("Choisissez une savegarde à afficher : ");
			_saveStore.DisplaySave(int.Parse(Console.ReadLine()));
        }

		private void EditerSauvegarde()
		{
            string newValue = string.Empty;
            _saveStore.DisplayAllSaves();
            Console.WriteLine("Choisissez une sauvegarde à éditer : ");
            int id = int.Parse(Console.ReadLine());
            _saveStore.DisplaySave(id);

            string property;
            while (true)
            {
                Console.WriteLine("Quelle propriété voulez-vous éditer ?");
                Console.WriteLine("1. Nom, 2. Type, 3. sourcePath, 4. destinationPath");
                string typeInput = Console.ReadLine();
                if (typeInput == "1")
                {
                    property = "Name";

                    break;
                }
                else if (typeInput == "2")
                {
                    property  = "Type";
                    break;
                }
                else if (typeInput == "3")
                {
                    property = "sourcePath";
                    do
                    {
                        Console.Write("Choisissez un chemin source pour la sauvegarde : ");
                        newValue = Console.ReadLine();
                        if (!Directory.Exists(newValue))
                        {
                            Console.WriteLine("Le chemin source n'existe pas. Veuillez entrer un chemin valide.");
                        }
                    } while (!Directory.Exists(newValue));
                    break;
                }
                else if (typeInput == "destinationPath")
                {
                    property = "destinationPath";
                    do
                    {
                        Console.Write("Choisissez un chemin source pour la sauvegarde : ");
                        newValue = Console.ReadLine();
                        if (!Directory.Exists(newValue))
                        {
                            Console.WriteLine("Le chemin source n'existe pas. Veuillez entrer un chemin valide.");
                        }
                    } while (!Directory.Exists(newValue));
                    break;
                }
                else
                {
                    Console.WriteLine("Type de sauvegarde invalide. Veuillez entrer 1 pour Full ou 2 pour Differential.");
                }
            }
            _saveStore.EditSave(id, property, newValue);
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
