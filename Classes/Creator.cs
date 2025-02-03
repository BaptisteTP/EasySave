using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public static class Creator
	{
		private static ResourceManager _resourceMangerInstance;

		public static ResourceManager GetResourceManagerInstance()
		{
			if( _resourceMangerInstance == null)
			{
				_resourceMangerInstance = new ResourceManager("Projet_Easy_Save.Resources.Strings", Assembly.GetExecutingAssembly());
			}
			return _resourceMangerInstance;
		}

		private static SaveStore _saveStoreInstance;
        public static SaveStore GetSaveStoreInstance()
		{
            if (_saveStoreInstance == null)
            {
                _saveStoreInstance = new SaveStore();
            }
			return _saveStoreInstance;

        }

		public static void GetPasterInstance()
		{

		}

		public static void GetLoggerInstance()
		{

		}
	}
}
