using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class Settings
	{
        // This class is responsible for managing the settings.
        public string? ActiveLanguage { get; set; }
		public string? FallBackLanguage { get; set; }
		public string? DailyLogPath { get; set; }
		public string? RealTimeLogPath { get; set; }
		public string? LogFormat { get; set; }

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
				LogFormat = ""
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

		private static void ChangeSetting(string setting, string newValue)
		{
            // Change a setting and write it to the json file.
            Settings currentSettings = Creator.GetSettingsInstance();
			
			switch (setting)
			{
				case "ActiveLanguage":
					currentSettings.ActiveLanguage = newValue;
					break;
				case "DailyLogPath":
					currentSettings.DailyLogPath = newValue;
					break;

				case "RealTimeLogPath":
					currentSettings.RealTimeLogPath = newValue;
					break;

				case "LogFormat":
					currentSettings.LogFormat = newValue;
					LogFomatChanged?.Invoke(null, EventArgs.Empty);
					break;

				default:
					return;
			}

			WriteSettingsToJsonFile(currentSettings);
		}

		public static void AskUserToChooseLanguage()
		{
            // Ask the user to choose a language.
            bool isSelectedLangageValid = false;
			Console.Clear();
			Console.WriteLine("Welcome/Bienvenue!");

			while (!isSelectedLangageValid)
			{
				Console.WriteLine();
				Console.WriteLine("1- Press 1 if you want to use this application in english");
				Console.WriteLine("2- Taper 2 si vous voulez utiliser cette application en français");
				Console.WriteLine();

				ConsoleKeyInfo keyChosen = Console.ReadKey();
				Console.Clear();

				if (keyChosen.KeyChar == '1')
				{
					ChangeSetting("ActiveLanguage", "en-US");
					ApplyLanguageSettings();
					break;
				}
				else if (keyChosen.KeyChar == '2')
				{
					ChangeSetting("ActiveLanguage", "fr-FR");
					ApplyLanguageSettings();
					break;
				}
				else
				{
					Console.WriteLine("The entered value is not valid/La valeur rentrée n'est pas valide !");
				}
			}
		}

		public static void ApplyLanguageSettings()
		{
            // Apply the language settings.
            Settings settings = Creator.GetSettingsInstance();
			if(settings.ActiveLanguage == "")
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(settings.FallBackLanguage!);
			}
			else
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(settings.ActiveLanguage!);
			}
		}

		public static void AskUserToChangeDailyLogsFolder()
		{
            // Ask the user to change the daily logs folder.
            Console.Clear();
			string currentFolderForDailyLog = GetBaseSettings().DailyLogPath!;

			string? newDailyLogFolderPath = string.Empty;
			while (string.IsNullOrEmpty(newDailyLogFolderPath) || !Directory.Exists(newDailyLogFolderPath) || !UserHasRightPermissionInFolder(newDailyLogFolderPath))
			{
				Console.WriteLine(_ressourceManager.GetString("InforUserDailyLogFile"));
				Console.WriteLine(currentFolderForDailyLog == "" ? _ressourceManager.GetString("NoFolder") : currentFolderForDailyLog);
				Console.WriteLine();
				Console.WriteLine(_ressourceManager.GetString("AskUser_NewDailyLogFolder"));
				newDailyLogFolderPath = Console.ReadLine();

				if (string.IsNullOrEmpty(newDailyLogFolderPath) || !Directory.Exists(newDailyLogFolderPath) || !UserHasRightPermissionInFolder(newDailyLogFolderPath))
				{
					Console.Clear();
					Console.WriteLine(_ressourceManager.GetString("InforUserInvalidPath"));
				}
			}

			GetBaseSettings().DailyLogPath = newDailyLogFolderPath;
			ChangeSetting("DailyLogPath", newDailyLogFolderPath!);
		}

		public static bool UserHasRightPermissionInFolder(string newDailyLogFolderPath)
		{
            // Check if the user has the right permissions in the folder.
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
		

		public static void AskUserToChangeRealTimeLogsFolder()
		{
            // Ask the user to change the real time logs folder.
            Console.Clear();
			string currentRealTimeLogPath = GetBaseSettings().RealTimeLogPath!;

			string? newRealTimeLogPath = string.Empty;
			while (string.IsNullOrEmpty(newRealTimeLogPath) || !Directory.Exists(newRealTimeLogPath) || !UserHasRightPermissionInFolder(newRealTimeLogPath))
			{
				Console.WriteLine(_ressourceManager.GetString("InforUserRealTimeLogFile"));
				Console.WriteLine(currentRealTimeLogPath == "" ? _ressourceManager.GetString("NoFolder") : currentRealTimeLogPath);
				Console.WriteLine();
				Console.WriteLine(_ressourceManager.GetString("AskUser_NewRealtimeLogFolder"));
				newRealTimeLogPath = Console.ReadLine();

				if (string.IsNullOrEmpty(newRealTimeLogPath) || !Directory.Exists(newRealTimeLogPath) || !UserHasRightPermissionInFolder(newRealTimeLogPath))
				{
					Console.Clear();
					Console.WriteLine(_ressourceManager.GetString("InforUserInvalidPath"));
				}
			}

			GetBaseSettings().RealTimeLogPath = newRealTimeLogPath;
			ChangeSetting("RealTimeLogPath", newRealTimeLogPath!);
		}

		public static void AskUserToChooseLogFormat()
		{
			Console.Clear();
			string currentLogFormat = GetBaseSettings().LogFormat!;

			int chosenFormat = -1;
			while (!IsChoosenLogFormatValid(chosenFormat))
			{
				Console.WriteLine(string.Format(_ressourceManager.GetString("InformUser_CurrentLogFormat"), currentLogFormat));
				bool isParseSuccessful = int.TryParse(Console.ReadKey().KeyChar.ToString(), out chosenFormat);

				if (!isParseSuccessful)
				{
					Console.Clear();
					Console.WriteLine(_ressourceManager.GetString("WrongCommandMessage"));
				}
			}
			LogFormat newLogFormat = Enum.Parse<LogFormat>(chosenFormat.ToString());
			ChangeSetting("LogFormat", newLogFormat.ToString());


		}

		private static bool IsChoosenLogFormatValid(int logFormat)
		{
			return Enum.IsDefined(typeof(LogFormat), logFormat);
		}
	}
}
