﻿using Project_Easy_Save.CustomEventArgs;
using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    public class Paster
    {
        public event EventHandler<CopyDirectoryEventArgs>? OnDirectoryCopied;
        public event EventHandler<FileCopyPreviewEventArgs>? OnFileCopyPreview;
        public event EventHandler<FileCopyEventArgs>? OnFileCopied;

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
            List<string> eligibleFiles = GetNameOfFilesModifiedAfterLastExecution(executedSave);

            if(eligibleFiles.Count == 0)
			{
				OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Inactive"));
				return false;
			}

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
            return true;
		}

        private List<string> GetNameOfFilesModifiedAfterLastExecution(Save executedSave)
        {
			List<string> result = new List<string>();

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
			List<string> eligibleFiles = GetEligibleFilesFullSave(executedSave.SourcePath);

			if(eligibleFiles.Count == 0)
			{
				OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Inactive"));
				return false;
			}

			List<string> remainingFiles = new List<string>(eligibleFiles);
			foreach (string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
            {
                CopyDirectory(executedSave, directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath));
            }

			foreach (string newPath in Directory.GetFiles(executedSave.SourcePath, "*.*", SearchOption.AllDirectories))
			{
				string destinationPath = newPath.Replace(executedSave.SourcePath, executedSave.DestinationPath);

				OnFileCopyPreview?.Invoke(this, new FileCopyPreviewEventArgs(executedSave, "Active", eligibleFiles, remainingFiles, newPath, destinationPath));
				CopyFile(newPath, executedSave, destinationPath);
				remainingFiles.Remove(newPath);
			}

			return true;
		}

		private List<string> GetEligibleFilesFullSave(string sourcePath)
		{
			List<string> result = new List<string>();

			foreach(string filePath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
			{
				result.Add(filePath);
			}

			return result;
		}

		private double GetPathSize()
        {
            return 0;
        }

        private void CopyDirectory(Save executedSave, string destPath)
        {
            var StopWatch = new Stopwatch();

            StopWatch.Start();
			Directory.CreateDirectory(destPath);
            StopWatch.Stop();

            OnDirectoryCopied?.Invoke(this, new CopyDirectoryEventArgs(DateTime.Now, executedSave, destPath, StopWatch.Elapsed));
		}

        private void CopyFile(string fileFullName, Save executedSave, string destinationPath)
        {
			var stopWatch = new Stopwatch();

            stopWatch.Start();
			File.Copy(fileFullName, destinationPath, true);
            stopWatch.Stop();

            OnFileCopied?.Invoke(this, new FileCopyEventArgs(DateTime.Now, executedSave, new FileInfo(fileFullName), destinationPath, stopWatch.Elapsed));
        }
    }
}
