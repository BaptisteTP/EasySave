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
            Console.Clear();
            IsInteracting = true;
            while (IsInteracting)
            {
                Console.WriteLine(_resourceManager.GetString("MessageBeforeExSaves"));
                ConsoleKeyInfo choice = Console.ReadKey(true);
                _saveStore.DisplayAllSaves();

                switch (choice.Key)
                {
                    case ConsoleKey.NumPad1:
                        ExAllSaves();
                        break;

                    case ConsoleKey.NumPad2:
                        ExSaves();
                        break;

                    case ConsoleKey.NumPad3:
                        Quit();
                        break;

                    case ConsoleKey.Escape:
                        Quit();
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
                        break;
                }
            }
        }

        private void ExAllSaves()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingAllSaveOperations"));
            _saveStore.GetAllSaves().ForEach(save => save.Execute());
            Console.WriteLine(_resourceManager.GetString("InformUser_return"));
            ConsoleKey hitKey = Console.ReadKey(true).Key;

            if (hitKey == "exit")
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
            Console.WriteLine(_resourceManager.GetString("InformUserReturnExit"));
            string saves = Console.ReadLine();

            if (saves == "exit")
            {
                Console.Clear();
                return;
            }
        }

        private void Quit()
        {
            IsInteracting = false;
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("InformUser_LeftExecutionPrompt"));
        }
    }
}

