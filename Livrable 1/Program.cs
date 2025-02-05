using Project_Easy_Save.Classes;
using System.Linq.Expressions;

class Program
{
    static void Main(string[] args)
    {
        Settings settings = Creator.GetSettingsInstance();

        if(settings.ActiveLanguage == "")
        {
            Settings.AskUserToChooseLanguage();
        }
        else
        {
            Settings.ApplyLanguageSettings();
        }

        MainMenuPrompt mainMenuPrompt = new MainMenuPrompt();
        mainMenuPrompt.Interact();

    }
}