using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class LanguageSettings
	{
		public void AskUserToChooseLanguage()
		{
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
					Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
					break;
				}
				else if (keyChosen.KeyChar == '2')
				{
					Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
					break;
				}
				else
				{
					Console.WriteLine("The entered value is not valid/La valeur rentrée n'est pas valide !");
				}
			}
		}
	}
}
