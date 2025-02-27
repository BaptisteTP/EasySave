using EasySave2._0.CustomEventArgs;
using EasySave2._0.Enums;
using EasySave2._0.Models.Notifications_Related;
using EasySave2._0.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace EasySave2._0.Models
{
	public class Paster
    {
        private static Paster? _pasterInstance;
		public event EventHandler<CopyDirectoryEventArgs>? OnDirectoryCopied;
		public event EventHandler<FileCopyPreviewEventArgs>? OnFileCopyPreview;
		public event EventHandler<FileCopyEventArgs>? OnFileCopied;
		public event EventHandler<Save>? SaveStarted;
		public event EventHandler<Save>? SaveFinished;
		public event EventHandler<Save>? BuisnessSoftwareDetected;

		private object _lockAddCriticalFile = new object();
		private object _lockRemoveCriticalFile = new object();

		public List<string> CriticalFiles = new List<string>();
		public bool CriticalFilesBeingCopied => CriticalFiles.Count > 0;

		private event EventHandler? CriticalFilesAdded;
		public event EventHandler? CriticalFilesCopyEnded;

        private Paster() { }

        public static Paster GetPasterInstance()
        {
            return _pasterInstance ??= new Paster();

		}
        
        public bool BeginCopyPaste(Save executedSave, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
            if (!Directory.Exists(executedSave.SourcePath)) { return false; }

			if(Creator.GetProcessObserverInstance().AnyBSOpened)
			{
				try
				{
					NotificationHelper.CreateNotifcation(title: Application.Current.Resources["BuisnessSoftwareTitle"].ToString(),
															content: Application.Current.Resources["BuisnessSofwareCancelSave"].ToString(),
															type: 1);

				}
				catch { }
				BuisnessSoftwareDetected?.Invoke(this, executedSave);
				return false; 
			}

			executedSave.Progress = 0;
			executedSave.IsExecuting = true;

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

		private void OnBuisnessSoftwareDetected(Save executedSave)
		{
			if (executedSave.IsExecuting == true && executedSave.IsPaused == false)
			{
				Creator.GetSaveStoreInstance().PauseSave(executedSave.Id,
                                                         wasSavePausedByUser: false);
			}
		}

		private void OnAllBuisnessSoftwareClosed(Save executedSave)
		{
			if(executedSave.IsPaused == true && executedSave.WasSavePausedByUser == false)
            {
				try
				{
					Creator.GetSaveStoreInstance().ResumeSave(executedSave.Id);
				}
				catch { }
            }
		}

		private bool BeginDifferentialSave(Save executedSave, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
			List<string> criticalFilesDetected = [];
			void BSDetected(object? sender, EventArgs e) => OnBuisnessSoftwareDetected(executedSave);
			void AllBSClosed(object? sender, EventArgs e) => OnAllBuisnessSoftwareClosed(executedSave);
			void CriticalFilesHandler(object? sender, EventArgs e) => HandleCriticalFiles(executedSave, criticalFilesDetected, cancellationToken, pauseEvent);
			void CriticalFilesCopyEndedHandler(object? sender, EventArgs e) => OnCriticalFilesCopyEnded(executedSave);

            try
            {
				List<string> eligibleFiles = GetNameOfFilesModifiedAfterLastExecution(executedSave);

				if (eligibleFiles.Count == 0)
				{
					return false;
				}

				ProcessObserver.BuisnessSoftwareDetected += BSDetected;
				ProcessObserver.AllBuisnessProcessClosed += AllBSClosed;
				CriticalFilesAdded += CriticalFilesHandler;
				CriticalFilesCopyEnded += CriticalFilesCopyEndedHandler;

				SaveStarted?.Invoke(this, executedSave);
				AddCriticalFiles(executedSave, eligibleFiles, out criticalFilesDetected);

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
							executedSave.Progress = Convert.ToInt32((1 - (double)(remainingFiles.Count - 1) / (double)(eligibleFiles.Count)) * 100);
						}
						catch
						{
							executedSave.Progress = 0;
						}
						OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, fileFullName, destinationPath));
						if (IsFileSizeWithinLimit(fileFullName) == true)
						{
							CopyFile(fileFullName, executedSave, destinationPath);
						}
						else
						{
							HandleOverLimitSize(fileFullName, executedSave, destinationPath);
						}
						remainingFiles.Remove(fileFullName);
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
							if (IsFileSizeWithinLimit(file.FullName) == true)
							{
								CopyFile(file.FullName, executedSave, destinationPath);
							}
							else
							{
								HandleOverLimitSize(file.FullName, executedSave, destinationPath);
							}
							remainingFiles.Remove(file.FullName);
						}
					}
				}

				executedSave.LastExecuteDate = DateTime.Now;

				SaveFinished?.Invoke(this, executedSave);
				executedSave.IsExecuting = false;
				executedSave.IsPaused = false;
				executedSave.Progress = 0;

				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				ProcessObserver.BuisnessSoftwareDetected -= BSDetected;
				ProcessObserver.AllBuisnessProcessClosed -= AllBSClosed;
				CriticalFilesAdded -= CriticalFilesHandler;
				CriticalFilesCopyEnded -= CriticalFilesCopyEndedHandler;

			}
        }
        private bool IsFileSizeWithinLimit(string fullName)
        {
            FileInfo fileInfo = new FileInfo(fullName);
            Settings settings = Creator.GetSettingsInstance();
            if (fileInfo.Length >= long.Parse(settings.FileSizeLimit) * 1024)
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
			List<string> criticalFilesDetected = [];
			void BSDetected(object? sender, EventArgs e) => OnBuisnessSoftwareDetected(executedSave);
			void AllBSClosed(object? sender, EventArgs e) => OnAllBuisnessSoftwareClosed(executedSave);
			void CriticalFilesHandler(object? sender, EventArgs e) => HandleCriticalFiles(executedSave, criticalFilesDetected, cancellationToken, pauseEvent);
			void CriticalFilesCopyEndedHandler(object? sender, EventArgs e) => OnCriticalFilesCopyEnded(executedSave);

			try
			{
				List<string> eligibleFiles = GetEligibleFilesFullSave(executedSave.SourcePath);

				if (eligibleFiles.Count == 0)
				{
					return false;
				}

				ProcessObserver.BuisnessSoftwareDetected += BSDetected;
				ProcessObserver.AllBuisnessProcessClosed += AllBSClosed;
				CriticalFilesAdded += CriticalFilesHandler;
				CriticalFilesCopyEnded += CriticalFilesCopyEndedHandler;

				SaveStarted?.Invoke(this, executedSave);
				AddCriticalFiles(executedSave, eligibleFiles, out criticalFilesDetected);

				SaveStore saveStore = Creator.GetSaveStoreInstance();
				int _nbFile = saveStore.CountFilesInDirectory(executedSave.SourcePath);

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
						executedSave.Progress = Convert.ToInt32((1 - (double)(remainingFiles.Count) / (double)(_nbFile)) * 100);
					}
					catch
					{
						executedSave.Progress = 0;
					}
					if (IsFileSizeWithinLimit(newPath) == true)
					{
						CopyFile(newPath, executedSave, destinationPath);
					}
					else
					{
						HandleOverLimitSize(newPath, executedSave, destinationPath);
					}
					remainingFiles.Remove(newPath);
				}

				executedSave.LastExecuteDate = DateTime.Now;

				SaveFinished?.Invoke(this, executedSave);
				executedSave.IsExecuting = false;
				executedSave.IsPaused = false;
				executedSave.Progress = 0;

				return true;
			}
			catch{
				return false;
			}
			finally
			{
				ProcessObserver.BuisnessSoftwareDetected -= BSDetected;
				ProcessObserver.AllBuisnessProcessClosed -= AllBSClosed;
				CriticalFilesAdded -= CriticalFilesHandler;
				CriticalFilesCopyEnded -= CriticalFilesCopyEndedHandler;
			}
		}

		private void OnCriticalFilesCopyEnded(Save executedSave)
		{
            if (executedSave.IsPaused && executedSave.WasSavePausedByUser == false)
            {
				try
				{
					Creator.GetSaveStoreInstance().ResumeSave(executedSave.Id);
				}
				catch { }
            }   
		}

		private void HandleCriticalFiles(Save executedSave, List<string> eligibleFiles, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
			List<string> saveFilesInCriticalFiles = CriticalFiles.Intersect(eligibleFiles).ToList();

            SaveStore saveStore = Creator.GetSaveStoreInstance();
            int _nbFile = saveStore.CountFilesInDirectory(executedSave.SourcePath);
            
			if(saveFilesInCriticalFiles.Count > 0 && !executedSave.IsCopyingCriticalFile)
            {
                executedSave.IsCopyingCriticalFile = true;
				executedSave.IsWaitingForCriticalFiles = false;
                int count = 1;

               foreach (string file in saveFilesInCriticalFiles)
               {
                    pauseEvent.Wait();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        foreach(string _file in saveFilesInCriticalFiles)
                        {
							RemoveCriticalFileFromList(_file);
						}
                        executedSave.IsCopyingCriticalFile = false;
						executedSave.IsWaitingForCriticalFiles = false;

						return;
					}

					FileInfo fileInfo = new FileInfo(file);

					string destDirectoryPath = fileInfo.Directory.FullName.Replace(executedSave.SourcePath, executedSave.DestinationPath);

                    if (!Directory.Exists(destDirectoryPath))
                    {
                        CopyDirectory(executedSave, destDirectoryPath);
                    }

                    string newPath = fileInfo.FullName;
                    string destinationPath = fileInfo.FullName.Replace(executedSave.SourcePath, executedSave.DestinationPath);
                    try
                    {
						executedSave.Progress = Convert.ToInt32(((double)(count) / (double)(_nbFile)) * 100);
					}
					catch
					{
						executedSave.Progress = 0;
					}

					OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, saveFilesInCriticalFiles, newPath, destinationPath));
					if (IsFileSizeWithinLimit(newPath) == true)
					{
						CopyFile(newPath, executedSave, destinationPath);
					}
					else
					{
						HandleOverLimitSize(newPath, executedSave, destinationPath);
					}

					RemoveCriticalFileFromList(file);
                    count++;

                }
                executedSave.IsCopyingCriticalFile = false;
				executedSave.IsWaitingForCriticalFiles = false;

			}
			if (!executedSave.IsCopyingCriticalFile && CriticalFiles.Count > 0)
			{
				executedSave.IsWaitingForCriticalFiles = true;
				Creator.GetSaveStoreInstance().PauseSave(executedSave.Id, false);
			}
		}

		private void RemoveCriticalFileFromList(string file)
		{
            lock (_lockRemoveCriticalFile)
            {
                CriticalFiles.Remove(file);
                if(CriticalFiles.Count == 0)
                {
					try
					{
						NotificationHelper.CreateNotifcation(title: Application.Current.Resources["CriticalFilesTitle"].ToString(),
															 content: Application.Current.Resources["CriticalFilesCopyEnd"].ToString(),
															 type: 2);

					}
					catch { }
					CriticalFilesCopyEnded?.Invoke(this, EventArgs.Empty);
				}
            }
		}

		private void AddCriticalFiles(Save executedSave, List<string> eligibleFiles, out List<string> detectedCriticalFiles)
        {

            detectedCriticalFiles = [];
            Settings settings = Creator.GetSettingsInstance();
            List<string> ExtensionPriority = settings.PriorityExtension;

			if(ExtensionPriority.Count == 0) { return; }
            bool wasCriticalFileAdded = false;

			foreach (string file in eligibleFiles)
            {
                foreach (string extension in ExtensionPriority)
                {
                    if (file.EndsWith(extension))
                    {
                        lock (_lockAddCriticalFile)
                        {
                            CriticalFiles.Add(file);
                        }
						detectedCriticalFiles.Add(file);
						wasCriticalFileAdded = true;
					}
                }
            }

            if(wasCriticalFileAdded)
			{
                foreach(string file in detectedCriticalFiles)
                {
                    eligibleFiles.Remove(file);
                }
			    CriticalFilesAdded?.Invoke(this, EventArgs.Empty);
            }
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

        public int NumberOfPriorityFiles;

		public int GetNumberOfPriorityFiles(Save executedSave)
        {
            Settings settings = Creator.GetSettingsInstance();
            List<string> ExtensionPriority = settings.PriorityExtension;
            List<string> eligibleFiles = GetNameOfFilesModifiedAfterLastExecution(executedSave);
            foreach (string file in eligibleFiles)
            {
                foreach (string extension in ExtensionPriority)
                {
                    if (file.EndsWith(extension))
                    {
                        NumberOfPriorityFiles++;
                    }
                }
            }

            return NumberOfPriorityFiles;
        }
        public List<string> GetPriorityFiles(Save executedSave)
        {
           
            return CriticalFiles;
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
            TimeSpan? timeEncrypt = null;
            var stopWatch = new Stopwatch();
            var TimeEncrypt = new Stopwatch();
            stopWatch.Start();

            try
            {
                File.Copy(fileFullName, destinationPath, true);

                if (executedSave.Encrypt)
                {
                    TimeEncrypt.Start();
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
                    TimeEncrypt.Stop();
                    timeEncrypt = TimeEncrypt.Elapsed;
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
                OnFileCopied?.Invoke(this, new FileCopyEventArgs(DateTime.Now, executedSave, new FileInfo(fileFullName), destinationPath, timeElapsed, timeEncrypt));
            }
        }

		#region Utilities

		#endregion
	}
}

