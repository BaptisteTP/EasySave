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

		public static void GetSaveStoreInstance()
		{

		}

		public static void GetPasterInstance()
		{

		}

		public static void GetLoggerInstance()
		{

		}
	}
}
