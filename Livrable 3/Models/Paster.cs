using EasySave2._0.CustomEventArgs;
using EasySave2._0.Enums;
using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave2._0.Models
{
    public class Paster
    {
		public event EventHandler<CopyDirectoryEventArgs>? OnDirectoryCopied;
		public event EventHandler<FileCopyPreviewEventArgs>? OnFileCopyPreview;
		public event EventHandler<FileCopyEventArgs>? OnFileCopied;
		public event EventHandler<Save>? SaveStarted;
		public event EventHandler<Save>? SaveFinished;
		public event EventHandler<Save>? BuisnessSoftwareDetected;

        public bool BeginCopyPaste(Save executedSave, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
            if (!Directory.Exists(executedSave.SourcePath)) { return false; }
            if (AreAnyBuisnessSoftwareUp()) { BuisnessSoftwareDetected?.Invoke(this, executedSave); return false; }

            switch (executedSave.Type)
            {
                case SaveType.Full:
                    return BeginFullSave(executedSave, cancellationToken, pauseEvent);

                case SaveType.Differential:
                    return BeginDifferentialSave(executedSave, cancellationToken, pauseEvent);

                default:
                    return false;
            }
        }

        private bool BeginDifferentialSave(Save executedSave, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
            List<string> eligibleFiles = GetNameOfFilesModifiedAfterLastExecution(executedSave);

            if (eligibleFiles.Count == 0)
            {
                return false;
            }

            SaveStarted?.Invoke(this, executedSave);
            List<string> remainingFiles = new List<string>(eligibleFiles);
            foreach (string fileFullName in Directory.GetFiles(executedSave.SourcePath))
            {
                if (cancellationToken.IsCancellationRequested) return false;
                pauseEvent.Wait(cancellationToken);

                if (eligibleFiles.Contains(fileFullName))
                {
                    string destinationPath = fileFullName.Replace(executedSave.SourcePath, executedSave.DestinationPath);
                    try
                    {
                        executedSave.Progress = Convert.ToInt32((1 - (double)(remainingFiles.Count - 1) / (double)(eligibleFiles.Count - 1)) * 100);
                    }
                    catch
                    {
                        executedSave.Progress = 0;
                    }
                    OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, fileFullName, destinationPath));
                    if (IsFileSizeWithinLimit(fileFullName) == true)
                    {
                        CopyFile(fileFullName, executedSave, destinationPath);
                        remainingFiles.Remove(fileFullName);
                    }
                    else
                    {
                        HandleOverLimitSize(fileFullName, executedSave, destinationPath);
                    }
                }
            }

            foreach (string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
            {
                if (cancellationToken.IsCancellationRequested) return false;
                pauseEvent.Wait(cancellationToken);

                DirectoryInfo directoryInfo = new DirectoryInfo(directorySourcePath);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    if (cancellationToken.IsCancellationRequested) return false;
                    pauseEvent.Wait(cancellationToken);

                    string destPath = directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath);
                    if (eligibleFiles.Contains(file.FullName))
                    {
                        if (!Directory.Exists(destPath))
                        {
                            CopyDirectory(executedSave, destPath);
                        }

                        string destinationPath = file.FullName.Replace(executedSave.SourcePath, executedSave.DestinationPath);

                        try
                        {
                            executedSave.Progress = Convert.ToInt32((1 - (double)(remainingFiles.Count - 1) / (double)(eligibleFiles.Count - 1)) * 100);
                        }
                        catch
                        {
                            executedSave.Progress = 0;
                        }
                        OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, file.FullName, destinationPath));
                        CopyFile(file.FullName, executedSave, destinationPath);
                        remainingFiles.Remove(file.FullName);
                    }
                }
            }
            executedSave.LastExecuteDate = DateTime.Now;
            SaveFinished?.Invoke(this, executedSave);
            return true;
        }
        private bool IsFileSizeWithinLimit(string fullName)
        {
            FileInfo fileInfo = new FileInfo(fullName);
            Settings settings = Creator.GetSettingsInstance();
            if (fileInfo.Length >= long.Parse(settings.FileSizeLimit) * 1000)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool BeginFullSave(Save executedSave, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
            List<string> eligibleFiles = GetEligibleFilesFullSave(executedSave.SourcePath);

            if (eligibleFiles.Count == 0)
            {
                return false;
            }

            SaveStarted?.Invoke(this, executedSave);
            List<string> remainingFiles = new List<string>(eligibleFiles);
            foreach (string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
            {
                if (cancellationToken.IsCancellationRequested) return false;
                pauseEvent.Wait(cancellationToken);

                CopyDirectory(executedSave, directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath));
            }
            foreach (string newPath in Directory.GetFiles(executedSave.SourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (cancellationToken.IsCancellationRequested) return false;
                pauseEvent.Wait(cancellationToken);

                string destinationPath = newPath.Replace(executedSave.SourcePath, executedSave.DestinationPath);

                OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, newPath, destinationPath));
                try
                {
                    executedSave.Progress = Convert.ToInt32((1 - (double)(remainingFiles.Count - 1) / (double)(eligibleFiles.Count - 1)) * 100);
                }
                catch
                {
                    executedSave.Progress = 0;
                }
                if (IsFileSizeWithinLimit(newPath) == true)
                {
                    CopyFile(newPath, executedSave, destinationPath);
                    remainingFiles.Remove(newPath);
                }
                else
                {
                    HandleOverLimitSize(newPath, executedSave, destinationPath);
                }
            }

            executedSave.LastExecuteDate = DateTime.Now;
            SaveFinished?.Invoke(this, executedSave);
            return true;
        }


        private List<string> GetNameOfFilesModifiedAfterLastExecution(Save executedSave)
		{
			List<string> result = new List<string>();
			// Get all files that have been modified since the last execution
			foreach (string filePath in Directory.GetFiles(executedSave.SourcePath, "*.*", SearchOption.AllDirectories))
			{
				FileInfo fileInfo = new FileInfo(filePath);
				if (executedSave.LastExecuteDate == null || fileInfo.LastWriteTime > executedSave.LastExecuteDate)
				{
					result.Add(fileInfo.FullName);
				}
			}

			return result;
		}

		private bool BeginFullSave(Save executedSave)
		{
			// Get all files in the source path
			List<string> eligibleFiles = GetEligibleFilesFullSave(executedSave.SourcePath);

			if (eligibleFiles.Count == 0)
			{
				return false;
			}

			SaveStarted?.Invoke(this, executedSave);
			List<string> remainingFiles = new List<string>(eligibleFiles);
			foreach (string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
			{
				CopyDirectory(executedSave, directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath));
			}
			// Copy all files
			foreach (string newPath in Directory.GetFiles(executedSave.SourcePath, "*.*", SearchOption.AllDirectories))
			{
				string destinationPath = newPath.Replace(executedSave.SourcePath, executedSave.DestinationPath);

				OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, newPath, destinationPath));
				try
				{
					executedSave.Progress = Convert.ToInt32((1 - (double)(remainingFiles.Count - 1) / (double)(eligibleFiles.Count - 1)) * 100);
				}
				catch
				{
					executedSave.Progress = 0;
                }
				CopyFile(newPath, executedSave, destinationPath);
				remainingFiles.Remove(newPath);
			}

			executedSave.LastExecuteDate = DateTime.Now;
			SaveFinished?.Invoke(this, executedSave);
			return true;
		}

        private readonly object _lock = new object();
        private void HandleOverLimitSize(string filePath, Save executedSave, string destinationPath)
        {
            lock (_lock)
            {
                CopyFile(filePath, executedSave, destinationPath);
            }
        }


        private List<string> GetEligibleFilesFullSave(string sourcePath)
		{
			// Get all files in the source path
			List<string> result = new List<string>();

			foreach (string filePath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
			{
				result.Add(filePath);
			}

			return result;
		}

		private void CopyDirectory(Save executedSave, string destPath)
		{
			// Create the directory in the destination path
			TimeSpan? timeElapsed = null;
			var StopWatch = new Stopwatch();
			StopWatch.Start();
			try
			{
				Directory.CreateDirectory(destPath);
				StopWatch.Stop();
				timeElapsed = StopWatch.Elapsed;

			}
			catch
			{
				StopWatch.Stop();
				timeElapsed = null;
			}
			finally
			{
				OnDirectoryCopied?.Invoke(this, new CopyDirectoryEventArgs(DateTime.Now, executedSave, destPath, timeElapsed));
			}

		}

        private void CopyFile(string fileFullName, Save executedSave, string destinationPath)
        {
            TimeSpan? timeElapsed = null;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                File.Copy(fileFullName, destinationPath, true);

                if (executedSave.Encrypt)
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "CryptoSoft.exe",
                        Arguments = $"{destinationPath} Baptiste",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var process = new Process { StartInfo = startInfo })
                    {
                        process.Start();
                        process.WaitForExit();

                        // Optionally, you can capture the output and error messages
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();

						if (process.ExitCode != 0)
						{
							// Handle the error case
							//throw new Exception($"CryptoSoft.exe a échoué avec le code de sortie {process.ExitCode}: {error}");
						}
					}
                }

                stopWatch.Stop();
                timeElapsed = stopWatch.Elapsed;
            }
            catch
            {
                stopWatch.Stop();
                timeElapsed = null;
            }
            finally
            {
                OnFileCopied?.Invoke(this, new FileCopyEventArgs(DateTime.Now, executedSave, new FileInfo(fileFullName), destinationPath, timeElapsed));
            }
           
        }

        private bool AreAnyBuisnessSoftwareUp()
        {
            Settings settings = Creator.GetSettingsInstance();
            Process[] processes = Process.GetProcesses();

            foreach (string buisnessSoftware in settings.BuisnessSoftwaresInterrupt)
            {
                foreach (var process in processes)
                {
                    if (process.ProcessName == buisnessSoftware)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

