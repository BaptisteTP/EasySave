using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class SettingsPrompt : PromptBase
	{
		private bool IsInteracting;

		public void Interact()
		{
			Console.Clear();
			IsInteracting = true;

			while (IsInteracting)
			{
				Console.WriteLine("Quel paramètre souhaitez-vous modifier ?");
				Console.WriteLine("\t1- Langue");
				Console.WriteLine("\t2- Répertoire des logs journaliers");
				Console.WriteLine("\t3- Répertoire des logs temps réels");

				ConsoleKey pressedKey = Console.ReadKey(true).Key;

				switch (pressedKey)
				{
					case ConsoleKey.NumPad1:
						Settings.AskUserToChooseLanguage();
						break;

					case ConsoleKey.NumPad2:
						Settings.AskUserToChangeDailyLogsFolder();
						break;

					case ConsoleKey.NumPad3:
						Settings.AskUserToChangeRealTimeLogsFolder();
						break;

					case ConsoleKey.Escape:
						Console.Clear();
						Console.WriteLine("Sortie de l'interface de modifications des paramètres");
						IsInteracting = false;
						break;

					default:
						Console.Clear();
						Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
						break;
				}
			}
		}
	}
}
