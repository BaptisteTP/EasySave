using EasySave2._0.ViewModels;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasySave2._0
{
    public partial class EncryptPage : Page
    {
        public EncryptPage()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() == true)
            {
                EncryptPath.Text = folderDialog.FolderName;
            }
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            string folderPath = EncryptPath.Text;
            string encryptionKey = Key.Text;

            if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(encryptionKey))
            {
                MessageBox.Show("Veuillez entrer à la fois le chemin du dossier et la clé de chiffrement.");
                return;
            }

            if (Directory.Exists(folderPath))
            {
                string decryptFolder = Path.Combine(folderPath, "Decrypt");

                if (!Directory.Exists(decryptFolder))
                {
                    Directory.CreateDirectory(decryptFolder);
                }

                var files = Directory.GetFiles(folderPath);

                foreach (var file in files)
                {
                    try
                    {
                        string fileName = Path.GetFileName(file);
                        string decryptedFilePath = Path.Combine(decryptFolder, fileName);

                        if (File.Exists(decryptedFilePath))
                        {
                            continue;
                        }

                        File.Copy(file, decryptedFilePath, overwrite: false);

                        EncryptFile(decryptedFilePath, encryptionKey);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la copie et du déchiffrement du fichier {file}: {ex.Message}");
                    }
                }

                MessageBox.Show("Déchiffrement terminé !");
            }
            else
            {
                MessageBox.Show("Le chemin du dossier est invalide.");
            }
        }

        private void EncryptFile(string filePath, string encryptionKey)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "CryptoSoft.exe";
                process.StartInfo.Arguments = $"\"{filePath}\" \"{encryptionKey}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show($"Fichier chiffré : {filePath}");
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution de CryptoSoft.exe : {ex.Message}");
            }
            stopwatch.Stop();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Creator.GetHomePageInstance());
        }
    }
}
