using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public abstract class PromptBase
	{
		protected ResourceManager _resourceManager;

		protected PromptBase()
		{
			_resourceManager = Creator.GetResourceManagerInstance();
		}
	}
}
