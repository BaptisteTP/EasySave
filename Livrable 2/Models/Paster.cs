using EasySave2._0.CustomEventArgs;
using EasySave2._0.Enums;
using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.Models
{
    class Paster
    {
		public event EventHandler<CopyDirectoryEventArgs>? OnDirectoryCopied;
		public event EventHandler<FileCopyPreviewEventArgs>? OnFileCopyPreview;
		public event EventHandler<FileCopyEventArgs>? OnFileCopied;
		public event EventHandler<Save>? SaveStarted;
		public event EventHandler<Save>? SaveFinished;

		public bool BeginCopyPaste(Save executedSave)
		{
			if (!Directory.Exists(executedSave.SourcePath)) { return false; }

			switch (executedSave.Type)
			{
				case SaveType.Full:
					return BeginFullSave(executedSave);

				case SaveType.Differential:
					return BeginDifferentialSave(executedSave);

				default:
					return false;
			}
		}

		private bool BeginDifferentialSave(Save executedSave)
		{
			// Get all files that have been modified since the last execution
			List<string> eligibleFiles = GetNameOfFilesModifiedAfterLastExecution(executedSave);

			if (eligibleFiles.Count == 0)
			{
				return false;
			}

			SaveStarted?.Invoke(this, executedSave);

			List<string> remainingFiles = new List<string>(eligibleFiles);
			foreach (string fileFullName in Directory.GetFiles(executedSave.SourcePath))
			{
				if (eligibleFiles.Contains(fileFullName))
				{
					string destinationPath = fileFullName.Replace(executedSave.SourcePath, executedSave.DestinationPath);

					OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, fileFullName, destinationPath));
					CopyFile(fileFullName, executedSave, destinationPath);
					remainingFiles.Remove(fileFullName);
				}
			}

			foreach (string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directorySourcePath);
				// Create the directory in the destination path and copy the files
				foreach (FileInfo file in directoryInfo.GetFiles())
				{
					string destPath = directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath);
					if (eligibleFiles.Contains(file.FullName))
					{
						if (!Directory.Exists(destPath))
						{
							CopyDirectory(executedSave, destPath);
						}

						string destinationPath = file.FullName.Replace(executedSave.SourcePath, executedSave.DestinationPath);

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
				CopyFile(newPath, executedSave, destinationPath);
				remainingFiles.Remove(newPath);
			}

			executedSave.LastExecuteDate = DateTime.Now;
			SaveFinished?.Invoke(this, executedSave);
			return true;
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
			// Copy the file to the destination path

			TimeSpan? timeElapsed = null;
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			try
			{
				File.Copy(fileFullName, destinationPath, true);
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
	}
}

