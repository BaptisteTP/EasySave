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
        // This class is responsible for managing the prompts.
        protected ResourceManager _resourceManager;
		protected SaveStore _saveStore; 

		protected PromptBase()
		{
			_resourceManager = Creator.GetResourceManagerInstance();
            _saveStore = Creator.GetSaveStoreInstance();
        }
	}
}
