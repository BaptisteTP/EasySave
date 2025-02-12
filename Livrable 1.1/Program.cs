using Project_Easy_Save.Classes;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Transactions;

class Program
{
    // Main function
    static void Main(string[] args)
	{
		//The app is lauched without any parameter (when double clicked on exe for example)
		if(args.Length == 0)
		{
			Settings settings = Creator.GetSettingsInstance();
			AskUserToCompleteSettings(settings);

			MainMenuPrompt mainMenuPrompt = new MainMenuPrompt();
			mainMenuPrompt.Interact();
		}
		//There are arguments (exe executed in command line)
		else if(args.Length == 1)
		{
			HandleCommandLineExecution(args);
		}
		else
		{
			Console.WriteLine("The specified parameters are not recongnized as valid.");
		}
	}

	private static void HandleCommandLineExecution(string[] args)
	{
		string input = args[0];
		if (Regex.IsMatch(input, @"^[1-9]\d*-\d+$"))
		{
			List<int> inputValues = GetInputValues(input);
			try
			{
				Creator.GetSaveStoreInstance().ExecuteSavesRange(inputValues[0], inputValues[1], OnSaveExecuted, OnFailedExecute);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		else if (Regex.IsMatch(input, @"^[1-9]\d*(;[1-9]\d*)*$"))
		{
			List<int> inputValues = GetInputValues(input);
			try
			{
				Creator.GetSaveStoreInstance().ExecuteSaves(inputValues, OnSaveExecuted, OnFailedExecute);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		else
		{
			Console.WriteLine($"Argmument {input} is not valid.");
		}
	}

	private static void OnSaveExecuted(int i)
	{
		Console.WriteLine($"Executing save {i}.");
	}

	private static void OnFailedExecute(int i)
	{
		Console.WriteLine($"Failed to execute save {i}...");
	}

	private static List<int> GetInputValues(string input)
	{
		string pattern = @"\d+";
		MatchCollection matches = Regex.Matches(input, pattern);

		List<int> numbers = [];
		foreach (Match match in matches)
		{
			numbers.Add(int.Parse(match.Value));
		}
		return numbers;
	}

	// Function that asks the user to complete the settings
	private static void AskUserToCompleteSettings(Settings settings)
	{
		if (settings.ActiveLanguage == "")
		{
			Settings.AskUserToChooseLanguage();
		}
		else
		{
			Settings.ApplyLanguageSettings();
		}

		if(settings.DailyLogPath == "" || !Directory.Exists(settings.DailyLogPath) || !Settings.UserHasRightPermissionInFolder(settings.DailyLogPath))
		{
			Settings.AskUserToChangeDailyLogsFolder();
		}

		if(settings.RealTimeLogPath == "" || !Directory.Exists(settings.RealTimeLogPath) || !Settings.UserHasRightPermissionInFolder(settings.RealTimeLogPath))
		{
			Settings.AskUserToChangeRealTimeLogsFolder();
		}

		if(settings.LogFormat == "")
		{
			Settings.AskUserToChooseLogFormat();
		}
	}
}