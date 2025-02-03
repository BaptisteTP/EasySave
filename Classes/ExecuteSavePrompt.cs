using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class ExecuteSavePrompt : PromptBase
	{
		private static bool IsInteracting;
		public ExecuteSavePrompt() : base() { }

		public static void Interact()
		{
			IsInteracting = true;
			while (IsInteracting)
			{

			}
		}
	}
}
