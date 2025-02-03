using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class MainMenuPrompt : PromptBase
	{
		private bool isInteracting;

		public MainMenuPrompt() : base() { }

		public void Interact()
		{
			EditSavesPrompt editSavesPrompt = new EditSavesPrompt();
			ExecuteSavePrompt executeSavePrompt = new ExecuteSavePrompt();
			isInteracting = true;

			Console.Clear();
			while (isInteracting)
			{
				Console.WriteLine("Que voulez-vous faire ?");
				Console.WriteLine("\t1- Editer des sauvegardes");
				Console.WriteLine("\t2- Exécuter des sauvegardes");
				Console.WriteLine("\t3- Changer de langue");
				Console.WriteLine("\t4- Quitter l'application");

				char optionSelected = Console.ReadKey(true).KeyChar;

				switch (optionSelected)
				{
					//Show edit prompt
					case '1':
						editSavesPrompt.Interact();
						break;

					//Show execute prompt
					case '2':
						executeSavePrompt.Interact();
						break;

					//Show language settings
					case '3':
						LanguageSettings.AskUserToChooseLanguage();
						break;

					//Leave app
					case '4':
						Environment.Exit(0);
						break;

					default:
						Console.Clear();
						Console.WriteLine("Commande non reconnue, veuillez réessayer.");
						break;
				}
			}
		}
	}
}
