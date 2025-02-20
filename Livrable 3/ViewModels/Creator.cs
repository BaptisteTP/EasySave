using EasySave2._0.Models;
using EasySave2._0.Models.Logs_Related;
using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EasySave2._0.ViewModels
{
    // Class that creates instances of the classes in the project
    public static class Creator
    {
        #region Views
        private static MainWindow _mainWindow;
        private static HomePage _homePage;
        private static AddSavePage _addSavePage;
        private static SettingPage _settingPage;
        private static WelcomePage _welcomePage;

		#endregion

		private static ResourceManager _resourceMangerInstance;
        private static SaveStore _saveStoreInstance;
        private static LogPage _logPage;
        private static Settings _settingsInstance;
        private static Paster _pasterInstance;
        private static Logger _loggerInstance;
        private static List<LanguageItem> _languages;

        // Returns a resource manager instance

        #region Get Views Instance

        public static MainWindow GetMainWindow()
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow();
            }
            return _mainWindow;
        }

        public static HomePage GetHomePageInstance() 
        {
            if (_homePage == null)
            {
                _homePage = new HomePage();
            }
            return _homePage;
        }
        public static SettingPage GetSettingPageInstance()
        {
            if (_settingPage == null)
            {
                _settingPage = new SettingPage();
            }
            return _settingPage;
        }

        public static AddSavePage GetAddSavePageInstance()
        {
            if(_addSavePage == null)
            {
				_addSavePage = new AddSavePage();
            }
            return _addSavePage;
        }

        public static LogPage GetLogPageInstance()
        {
            if (_logPage == null)
            {
                _logPage = new LogPage();
            }
            return _logPage;
        }

        public static WelcomePage GetWelcomePage()
        {
            if( _welcomePage == null)
            {
                _welcomePage = new WelcomePage();
            }
            return _welcomePage;
        }

		#endregion

        public static List<LanguageItem> GetAvalaibleLanguages()
        {
            if(_languages == null)
            {
                _languages = new List<LanguageItem>()
                {
					new LanguageItem() { Image = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "assets/images/FrenchButton.png"), UriKind.Absolute)) , Text = "Français", Language = "fr-FR" },
			        new LanguageItem() { Image = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "assets/images/EnglishButton.png"), UriKind.Absolute)), Text = "English", Language = "en-US" },
				};
            }
            return _languages;
        }

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

                _pasterInstance = new Paster();
                _loggerInstance = new Logger();

                _pasterInstance.OnFileCopyPreview += _loggerInstance.OnCopyFilePreview;
                _pasterInstance.OnFileCopied += _loggerInstance.OnCopyFile;
				_pasterInstance.OnDirectoryCopied += _loggerInstance.OnCopyDirectory;
				_pasterInstance.SaveStarted += _loggerInstance.OnSaveStarted;
				_pasterInstance.SaveFinished += _loggerInstance.OnSaveFinished;
                _pasterInstance.BuisnessSoftwareDetected += _loggerInstance.OnBuisnessSoftwareDetected;
				_saveStoreInstance.SaveCreated += _loggerInstance.OnSaveCreated;
				_saveStoreInstance.SaveEdited += _loggerInstance.OnSaveEdited;
				_saveStoreInstance.SaveDeleted += _loggerInstance.OnSaveDeleted;
				Settings.LogFomatChanged += _loggerInstance.OnSaveCreated;

				_saveStoreInstance.LoadLoggedSaves();
			}
            return _saveStoreInstance;

        }

        // Returns a paster instance
        public static Paster GetPasterInstance()
        {
            if (_pasterInstance == null)
            {
                throw new Exception("Error");
            }
            return _pasterInstance;
        }

        // Returns a logger instance
        public static Settings GetSettingsInstance()
        {
            if (_settingsInstance == null)
            {
                string appsettings = "appsettings.json";
                if (!File.Exists(appsettings))
                {
                    _settingsInstance = Settings.CreateBaseSettings();
                }
                else
                {
                    _settingsInstance = Settings.GetBaseSettings();
                }
            }
            return _settingsInstance;
        }
    }
}