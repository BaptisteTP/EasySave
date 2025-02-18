
using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave2._0.Models
{
	public class Settings
	{
		public string? ActiveLanguage { get; set; }
		public string? FallBackLanguage { get; set; }
		public string? DailyLogPath { get; set; }
		public string? RealTimeLogPath { get; set; }
		public string? LogFormat { get; set; }
		public List<string> BuisnessSoftwaresInterrupt { get; set; }

		public static event EventHandler? LogFomatChanged;

		private static ResourceManager _ressourceManager = Creator.GetResourceManagerInstance();

		public static Settings CreateBaseSettings()
		{
			// Create a base settings file with default values.
			Settings baseSettings = new Settings()
			{
				ActiveLanguage = "",
				FallBackLanguage = "en-US",
				DailyLogPath = "",
				RealTimeLogPath = "",
				LogFormat = "",
				BuisnessSoftwaresInterrupt = new List<string>(),
			};
			WriteSettingsToJsonFile(baseSettings);

			return baseSettings;
		}

		private static void WriteSettingsToJsonFile(Settings settings)
		{
			// Write the settings to a json file.
			var options = new JsonSerializerOptions { WriteIndented = true };
			string baseSttingsJson = JsonSerializer.Serialize<Settings>(settings, options);

			using (StreamWriter sw = File.CreateText("appsettings.json"))
			{
				sw.WriteLine(baseSttingsJson);
			}
		}

		public static Settings GetBaseSettings()
		{
			// Get the settings from the json file.
			try
			{
				string jsonAppSettings = File.ReadAllText("appsettings.json");
				Settings foundSettings = JsonSerializer.Deserialize<Settings>(jsonAppSettings);
				return foundSettings;
			}
			catch
			{
				return CreateBaseSettings();
			}
		}

		public static void ChangeSetting(string setting, object newValue)
		{
			// Change a setting and write it to the json file.
			Settings currentSettings = Creator.GetSettingsInstance();

			switch (setting)
			{
				case "ActiveLanguage":
					currentSettings.ActiveLanguage = (string)newValue;
					break;
				case "DailyLogPath":
					currentSettings.DailyLogPath = (string)newValue;
					break;

				case "RealTimeLogPath":
					currentSettings.RealTimeLogPath = (string)newValue;
					break;

				case "LogFormat":
					currentSettings.LogFormat = (string)newValue;
					LogFomatChanged?.Invoke(null, EventArgs.Empty);
					break;

				case "BuisnessSoftwaresInterrupt":
					currentSettings.BuisnessSoftwaresInterrupt = (List<string>)newValue;
					break;
				default:
					return;
			}

			WriteSettingsToJsonFile(currentSettings);
		}

        public static void ApplyLanguageSettings()
        {
            // Apply the language settings changes.
			Settings settings = Creator.GetSettingsInstance();
            ResourceDictionary dictionary = new ResourceDictionary();
            switch (settings.ActiveLanguage)
            {
                case "en-US":
                    dictionary.Source = new Uri("../Resources/StringsResources.en-US.xaml", UriKind.Relative);
                    break;
                case "fr-FR":
                    dictionary.Source = new Uri("../Resources/StringsResources.fr-FR.xaml", UriKind.Relative);
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Add(dictionary);

        }

        public static bool UserHasRightPermissionInFolder(string newDailyLogFolderPath)
		{
			// Check if the user has the right permissions in the folder.
			if (!Directory.Exists(newDailyLogFolderPath)) {  return false; }

			try
			{
				using FileStream fs = File.Create(
					Path.Combine(
						newDailyLogFolderPath,
						Path.GetRandomFileName()
					),
					1,
					FileOptions.DeleteOnClose);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}

