using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Project_Easy_Save.Classes
{
    // Class that prompts the user to execute the saves
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

        // Method that executes all the saves
        private void ExAllSaves()
        {
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingAllSaveOperations"));
            _saveStore.GetAllSaves().ForEach(save => save.Execute());
            Console.WriteLine(_resourceManager.GetString("MessageAfterShowingAllSaveOperations"));

        }

        // Method that executes the saves
        private void ExSaves()
        {
            int choice;
            bool isimputvalid = false;
            Save save = null;
            Console.Clear();
            while (isimputvalid == false || save == null) 
            {
                _saveStore.DisplayAllSaves();
                Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingExSaves"));
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    Console.Clear();
                    return;
                }
                isimputvalid = int.TryParse(input, out choice);
                save = _saveStore.GetSave(choice);
                Console.Clear();
                if (isimputvalid == false || save == null)
                {
                    Console.WriteLine(_resourceManager.GetString("WrongCommandMessage"));
                }
            }
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingSaveOperations"));
            save.Execute();
            Console.WriteLine(_resourceManager.GetString("MessageAfterShowingAllSaveOperations"));
        }

        // Method that quits the execution
        private void Quit()
        {
            IsInteracting = false;
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("InformUser_LeftExecutionPrompt"));
        }
    }
}

