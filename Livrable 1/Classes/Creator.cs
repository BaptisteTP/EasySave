using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public static class Creator
	{
		private static ResourceManager _resourceMangerInstance;
		private static SaveStore _saveStoreInstance;
		private static Paster _pasterInstance;
		private static Settings _settingsInstance;

		public static ResourceManager GetResourceManagerInstance()
		{
			if( _resourceMangerInstance == null)
			{
				_resourceMangerInstance = new ResourceManager("Project_Easy_Save.Resources.Strings", Assembly.GetExecutingAssembly());
			}
			return _resourceMangerInstance;
		}

        public static SaveStore GetSaveStoreInstance()
		{
            if (_saveStoreInstance == null)
            {
                _saveStoreInstance = new SaveStore();
            }
			return _saveStoreInstance;

        }

		public static Paster GetPasterInstance()
		{
			if(_pasterInstance == null)
			{
				_pasterInstance = new Paster();
			}
			return _pasterInstance;
		}

		public static void GetLoggerInstance()
		{

		}

		public static Settings GetSettingsInstance()
		{
			if( _settingsInstance == null)
			{
				string appsettings = "appsettings.json";
				string jsonAppSettings = File.ReadAllText(appsettings);
				_settingsInstance = JsonSerializer.Deserialize<Settings>(jsonAppSettings);
			}
			return _settingsInstance;
		}
	}
}
