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
						CreerSauvegarde();
						break;

					case '2':
						AfficherSauvegarde();
						break;

					case '3':
						EditerSauvegarde();
						break;

					case '4':
						SupprimerSauvegarde();
						break;

					case '5':
						Quitter();
						break;
				}
			}
		}

		private void AfficherMenu()
		{
			Console.Clear();
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

		private void CreerSauvegarde()
		{
			Console.Clear();
			Console.WriteLine("Création d'une sauvegarde");
		}

		private void AfficherSauvegarde()
		{
			Console.Clear();
			Console.WriteLine("Affichage d'une sauvegarde");
		}

		private void EditerSauvegarde()
		{
			Console.Clear();
			Console.WriteLine("Édition d'une sauvegarde");
		}

		private void SupprimerSauvegarde()
		{
			Console.Clear();
			Console.WriteLine("Suppression d'une sauvegarde");
		}

		private void Quitter()
		{
			IsInteracting = false;
			Console.Clear();
			Console.WriteLine("Vous avez quitté le menu d'édtion des sauvegardes.");
		}
	}
}
