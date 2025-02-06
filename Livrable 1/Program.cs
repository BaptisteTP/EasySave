﻿using Project_Easy_Save.Classes;
using System.Linq.Expressions;

class Program
{
    static void Main(string[] args)
	{
		Settings settings = Creator.GetSettingsInstance();
		AskUserToCompleteSettings(settings);

		MainMenuPrompt mainMenuPrompt = new MainMenuPrompt();
		mainMenuPrompt.Interact();

	}

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

		if(settings.DailyLogPath == "" || !Directory.Exists(settings.DailyLogPath))
		{
			Settings.AskUserToChangeDailyLogsFolder();
		}

		if(settings.RealTimeLogPath == "" || !Directory.Exists(settings.RealTimeLogPath))
		{
			Settings.AskUserToChangeRealTimeLogsFolder();
		}
	}
}