using Project_Easy_Save.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LogLib;

namespace Project_Easy_Save.Classes
{
	public class Logger
	{
        private LogLib.Logger LogLibLogger;
        public Logger()
		{
			LogLibLogger = new LogLib.Logger();
		}

		public void OnCopyDirectory(object sender, CopyDirectoryEventArgs eventArgs)
		{
			DirectoryCopyLog log = new DirectoryCopyLog(name: eventArgs.ExecutedSave.Name,
														directoryTransferTime: eventArgs.CreationTimeSpan,
														time: eventArgs.CopyDate);

			string jsonToLog = SerializeObjectToJson(log);
			string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

			//Log with logger lib
			LogLibLogger.WriteLog(log, pathToWriteTo);
		}

		public void OnCopyFile(object sender, FileCopyEventArgs eventArgs)
		{
			FileCopyLog log = new FileCopyLog(name: eventArgs.ExecutedSave.Name,
											  fileSource: eventArgs.SourceFile.FullName,
											  fileTarget: eventArgs.DestinationPath,
											  fileSize: eventArgs.FileSize,
											  fileTransferTime: eventArgs.TransferTime,
											  time: eventArgs.CopyDate);

			string jsonToLog = SerializeObjectToJson(log);
			string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

			//Log with logger lib
			LogLibLogger.WriteLog(log, pathToWriteTo);
        }

		public void OnCopyFilePreview(object sender, FileCopyPreviewEventArgs eventArgs)
		{
			FileCopyPreviewLog log = new FileCopyPreviewLog(name: eventArgs.ExecutedSave.Name,
															sourceFilePath: eventArgs.CurrentFileSourcePath,
															targetFilePath: eventArgs.CurrentFileDestinationPath,
															state: eventArgs.State,
															totalFileToCopy: eventArgs.EligibleFiles.Count,
															totalFileSize: 19,
															nbFilesLeftToDo: eventArgs.RemainingFiles.Count);

			string jsonToLog = SerializeObjectToJson(log);
			string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

            //Log with logger lib
        }

		private string SerializeObjectToJson(object log)
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			return JsonSerializer.Serialize(log, options);
		}
	}
}
