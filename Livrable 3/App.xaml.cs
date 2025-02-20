using EasySave2._0.Models;
using EasySave2._0.ViewModels;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;

namespace EasySave2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex = new Mutex(true, "{E32D87C6-33A6-4F9D-A8D2-3F2C0A00F4F3}");

        async void App_Startup(object sender, StartupEventArgs e)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("L'application est déjà en cours d'exécution.");
                App.Current.Shutdown();
                return;
            }

            if (e.Args.Length == 0)
            {
                Settings.ApplyLanguageSettings();
                MainWindow mainWindow = Creator.GetMainWindow();
                mainWindow.Show();
                mainWindow.StartAppNaviguation();
            }
            else if (e.Args.Length == 1)
            {
                await HandleCommandLineExecution(e.Args);
                App.Current.Shutdown();
            }
            else
            {
                Console.WriteLine("Les paramètres spécifiés ne sont pas reconnus comme valides.");
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
                Console.WriteLine($"L'argument {input} n'est pas valide.");
            }
        }

        private static void OnSaveExecuted(int i)
        {
            Console.WriteLine($"Exécution du sauvegarde {i}.");
        }

        private static void OnFailedExecute(int i)
        {
            Console.WriteLine($"Échec de l'exécution de la sauvegarde {i}...");
        }

        private static List<int> GetInputValues(string input)
        {
            string pattern = @"\d+";
            MatchCollection matches = Regex.Matches(input, pattern);

            List<int> numbers = new List<int>();
            foreach (Match match in matches)
            {
                numbers.Add(int.Parse(match.Value));
            }
            return numbers;
        }
        protected override void OnExit(ExitEventArgs e)
        {
            mutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
