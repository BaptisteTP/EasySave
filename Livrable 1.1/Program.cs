using Project_Easy_Save.Classes;
using System.Linq.Expressions;
using System.Transactions;

class Program
{
    // Main function
    static void Main(string[] args)
	{
		Settings settings = Creator.GetSettingsInstance();
		AskUserToCompleteSettings(settings);

		MainMenuPrompt mainMenuPrompt = new MainMenuPrompt();
		mainMenuPrompt.Interact();

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