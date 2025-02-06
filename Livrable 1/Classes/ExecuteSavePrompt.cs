using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class ExecuteSavePrompt : PromptBase
	{
		private bool IsInteracting;
		public ExecuteSavePrompt() : base() { }

        public void Interact()
        {
            IsInteracting = true;
            while (IsInteracting)
            {
                Console.Clear();
                Console.WriteLine(_resourceManager.GetString("MessageBeforeExSaves"));
                ConsoleKeyInfo choice = Console.ReadKey(true);
                _saveStore.DisplayAllSaves();

                switch (choice.KeyChar)
                {
                    case '1':
                        ExAllSaves();
                        break;
                    case '2':
                        ExSaves();
                        break;
                    case '3':
                        Quit();
                        break;
                }
            }
        }

        private void ExAllSaves()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingAllSaveOperations"));
            _saveStore.DisplayAllSaves();
            Console.WriteLine(_resourceManager.GetString("InformUser_return"));
            ConsoleKey hitKey = Console.ReadKey(true).Key;

            if (hitKey == ConsoleKey.Escape)
            {
                Console.Clear();
                return;
            }
        }
        private void ExSaves()
        {
            Console.Clear();
            _saveStore.DisplayAllSaves();
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingExSaves"));
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("InformUser_return"));
            ConsoleKey hitKey = Console.ReadKey(true).Key;

            if (hitKey == ConsoleKey.Escape)
            {
                Console.Clear();
                return;
            }
        }

        private void Quit()
        {
            IsInteracting = false;
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("InformUser_SaveOperationFormQuit"));
        }
    }
}

