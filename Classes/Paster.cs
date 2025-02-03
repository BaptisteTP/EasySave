using Project_Easy_Save.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    public class Paster
    {
        public event EventHandler? OnSaveExecute;
        public event EventHandler? OnDirectoryCopy;
        public event EventHandler? OnFileCopy;

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

            if(eligibleFiles.Count == 0) { return false; }

			foreach (string fileFullName in Directory.GetFiles(executedSave.SourcePath))
			{
				if (eligibleFiles.Contains(fileFullName))
				{
					CopyFile(fileFullName, executedSave.SourcePath, executedSave.DestinationPath);
				}
			}

			foreach (string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
			{
                string destPath = directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath);
				DirectoryInfo directoryInfo = new DirectoryInfo(directorySourcePath);

				foreach (FileInfo file in directoryInfo.GetFiles())
                {
					if (eligibleFiles.Contains(file.FullName))
                    {
						if (!Directory.Exists(destPath))
						{
							CopyDirectory(destPath);
						}

						CopyFile(file.FullName, directorySourcePath, destPath);
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
            foreach(string directorySourcePath in Directory.GetDirectories(executedSave.SourcePath, "*", SearchOption.AllDirectories))
            {
                CopyDirectory(directorySourcePath.Replace(executedSave.SourcePath, executedSave.DestinationPath));
            }

			foreach (string newPath in Directory.GetFiles(executedSave.SourcePath, "*.*", SearchOption.AllDirectories))
			{
                CopyFile(newPath, executedSave.SourcePath, executedSave.DestinationPath);
			}

			return true;
		}

		private double GetPathSize()
        {
            return 0;
        }

        private void CopyDirectory(string destPath)
        {
			Directory.CreateDirectory(destPath);
            OnDirectoryCopy?.Invoke(this, new EventArgs());
		}

        private void CopyFile(string fileFullName, string sourcePath, string destPath)
        {
			File.Copy(fileFullName, fileFullName.Replace(sourcePath, destPath), true);
            OnFileCopy?.Invoke(this, new EventArgs());
        }

        //public List<string> SearchAllPathFiles(string sourcePath, string basePath)
        //{
        //    List<string> allFiles = new List<string>();
        //    if (!Directory.Exists(sourcePath))
        //    {
        //        Console.WriteLine($"The directory '{sourcePath}' does not exist.");
        //        return null;
        //    }

        //    string[] files = Directory.GetFiles(sourcePath);
        //    string[] directories = Directory.GetDirectories(sourcePath);

        //    foreach (string file in files)
        //    {
        //        string relativePath = Path.GetRelativePath(basePath, file);
        //        allFiles.Add(relativePath);
        //    }

        //    foreach (string directory in directories)
        //    {
        //        SearchAllPathFiles(directory, basePath); // Recursively search in subdirectories
        //    }
        //    return allFiles;
        //}
        //public bool BeginCopyPasteFull(Save save, String allfile)
        //{
        //    try
        //    {
        //        List<string> allFiles = SearchAllPathFiles(save.SourcePath, save.SourcePath);
        //        foreach (string file in allFiles)
        //        {
        //            File.Copy(save.SourcePath + file, save.DestinationPath + file);
        //        }
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return false;

        //    }
        //}
    }
}
