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
            //_saveStore.GetAllSaves().ForEach(save => save.Execute());
            while (IsInteracting)
            {
                Console.Clear();
                Console.WriteLine(_resourceManager.GetString("MessageBeforeExSaves"));
                ConsoleKeyInfo choix = Console.ReadKey(true);
                _saveStore.DisplayAllSaves();

                switch (choix.KeyChar)
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
            Console.WriteLine("Execution de toutes les sauvegardes");
            _saveStore.GetAllSaves().ForEach(save => save.Execute());
        }
        private void ExSaves()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("MessageBeforeShowingExSaves"));
            _saveStore.DisplayAllSaves();
            _saveStore.GetSave(int.Parse(Console.ReadLine()));
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("Quit"));
        }

        private void Quit()
        {
            IsInteracting = false;
            Console.Clear();
            Console.WriteLine(_resourceManager.GetString("InformUser_SaveOperationFormQuit"));
        }
    }
}

