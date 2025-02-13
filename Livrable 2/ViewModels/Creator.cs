using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave2._0.ViewModels
{
    // Class that creates instances of the classes in the project
    public static class Creator
    {
        private static ResourceManager _resourceMangerInstance;
        private static SaveStore _saveStoreInstance;
        //private static Paster _pasterInstance;
        //private static Settings _settingsInstance;
        //private static Logger _loggerInstance;

        // Returns a resource manager instance
        public static ResourceManager GetResourceManagerInstance()
        {
            if (_resourceMangerInstance == null)
            {
                _resourceMangerInstance = new ResourceManager("Project_Easy_Save.Resources.Strings", Assembly.GetExecutingAssembly());
            }
            return _resourceMangerInstance;
        }

        // Returns a save store instance
        public static SaveStore GetSaveStoreInstance()
        {
            if (_saveStoreInstance == null)
            {
                _saveStoreInstance = new SaveStore();

                //_pasterInstance = new Paster();
                //_loggerInstance = new Logger();

                //_pasterInstance.OnFileCopyPreview += _loggerInstance.OnCopyFilePreview;
                //_pasterInstance.OnFileCopied += _loggerInstance.OnCopyFile;
                //_pasterInstance.OnDirectoryCopied += _loggerInstance.OnCopyDirectory;
                //_pasterInstance.SaveFinished += _loggerInstance.OnSaveFinished;
                //_saveStoreInstance.SaveCreated += _loggerInstance.OnSaveCreated;
            }
            return _saveStoreInstance;

        }

        // Returns a paster instance
        //public static Paster GetPasterInstance()
        //{
        //    if (_pasterInstance == null)
        //    {
        //        throw new Exception("Error");
        //    }
        //    return _pasterInstance;
        //}

        //// Returns a logger instance
        //public static Settings GetSettingsInstance()
        //{
        //    if (_settingsInstance == null)
        //    {
        //        string appsettings = "appsettings.json";
        //        if (!File.Exists(appsettings))
        //        {
        //            _settingsInstance = Settings.CreateBaseSettings();
        //        }
        //        else
        //        {
        //            _settingsInstance = Settings.GetBaseSettings();
        //        }
        //    }
        //    return _settingsInstance;
        //}
    }
}