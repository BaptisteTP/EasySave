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

			}
		}
	}
}
