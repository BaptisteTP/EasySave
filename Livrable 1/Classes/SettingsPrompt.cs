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
				Console.WriteLine(_resourceManager.GetString("AskUserChangeSetting"));

				ConsoleKey pressedKey = Console.ReadKey(true).Key;

				switch (pressedKey)
				{
					case ConsoleKey.NumPad1:
						Settings.AskUserToChooseLanguage();
						Console.Clear();
						break;

					case ConsoleKey.NumPad2:
						Settings.AskUserToChangeDailyLogsFolder();
						Console.Clear();
						break;

					case ConsoleKey.NumPad3:
						Settings.AskUserToChangeRealTimeLogsFolder();
						Console.Clear();
						break;

					case ConsoleKey.Escape:
						Console.Clear();
						Console.WriteLine(_resourceManager.GetString("InformUserOutSettingInterface"));
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
