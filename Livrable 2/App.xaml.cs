using EasySave2._0.Models;
using EasySave2._0.ViewModels;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;

namespace EasySave2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		async void App_Startup(object sender, StartupEventArgs e)
		{
			//The app is lauched without any parameter (when double clicked on exe for example)
			if (e.Args.Length == 0)
			{
				Settings.ApplyLanguageSettings();
				MainWindow mainWindow = Creator.GetMainWindow();
				mainWindow.Show();
				mainWindow.StartAppNaviguation();
			}
			//There are arguments (exe executed in command line)
			else if (e.Args.Length == 1)
			{
				await HandleCommandLineExecution(e.Args);
				App.Current.Shutdown();
			}
			else
			{
				Console.WriteLine("The specified parameters are not recongnized as valid.");
				App.Current.Shutdown();
			}
		}

		private static async Task HandleCommandLineExecution(string[] args)
		{
			string input = args[0];
			if (Regex.IsMatch(input, @"^[1-9]\d*-\d+$"))
			{
				List<int> inputValues = GetInputValues(input);
				try
				{
					await Creator.GetSaveStoreInstance().ExecuteSavesRange(inputValues[0], inputValues[1], OnSaveExecuted, OnFailedExecute);
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
					await Creator.GetSaveStoreInstance().ExecuteSaves(inputValues, OnSaveExecuted, OnFailedExecute);
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
	}

}
