using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Resources;
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
            SettingsPrompt settingsPrompt = new SettingsPrompt();
            isInteracting = true;

            Console.Clear();
            while (isInteracting)
            {
                Console.WriteLine(_resourceManager.GetString("AskForUserActionInMainMenu"));

                char optionSelected = Console.ReadKey(true).KeyChar;

                switch (optionSelected)
                {
                    //Show edit prompt
                    case '1':
                        editSavesPrompt.Interact();
                        break;

					//Show execute prompt
					case '2':
						HandleSaveExecution();
						break;

					//Show settings prompt
					case '3':
                        settingsPrompt.Interact();
                        break;

                    //Leave app
                    case '4':
                        Environment.Exit(0);
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
                        break;
                }
            }
        }

		private void HandleSaveExecution()
		{
			if (_saveStore.CanExecuteSave)
			{
				ExecuteSavePrompt executeSavePrompt = new ExecuteSavePrompt();
				executeSavePrompt.Interact();
			}
			else
			{
				Console.Clear();
				Console.WriteLine(_resourceManager.GetString("NoOperationInStoreMessage"));
			}
		}
	}
}