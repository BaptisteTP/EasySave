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
        // This class is responsible for logging the events.
        private LogLib.Logger LogLibLogger;
        public Logger()
		{
			LogLibLogger = new LogLib.Logger();
		}

		public void OnCopyDirectory(object sender, CopyDirectoryEventArgs eventArgs)
		{
            // Create a new DirectoryCopyLog object
            DirectoryCopyLog log = new DirectoryCopyLog(name: eventArgs.ExecutedSave.Name,
														directoryTransferTime: eventArgs.CreationTimeSpan,
														time: eventArgs.CopyDate);

			string jsonToLog = SerializeObjectToJson(log);
			string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

			//Log with logger lib
			LogLibLogger.WriteDailyLog(jsonToLog, pathToWriteTo);
		}

		public void OnCopyFile(object sender, FileCopyEventArgs eventArgs)
		{
            // Create a new FileCopyLog object
            FileCopyLog log = new FileCopyLog(name: eventArgs.ExecutedSave.Name,
											  fileSource: eventArgs.SourceFile.FullName,
											  fileTarget: eventArgs.DestinationPath,
											  fileSize: eventArgs.FileSize,
											  fileTransferTime: eventArgs.TransferTime,
											  time: eventArgs.CopyDate);

			string jsonToLog = SerializeObjectToJson(log);
			string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

			//Log with logger lib
			LogLibLogger.WriteDailyLog(jsonToLog, pathToWriteTo);
        }

		public void OnCopyFilePreview(object sender, FileCopyPreviewEventArgs eventArgs)
		{
            // Write on json file for time log
            string pathToRealTimeLog = Path.Combine(Creator.GetSettingsInstance().RealTimeLogPath, "realTimeLog.json");
			string jsonString = File.ReadAllText(pathToRealTimeLog);
			List<FileCopyPreviewLog> logList = JsonSerializer.Deserialize<List<FileCopyPreviewLog>>(jsonString);

			FileCopyPreviewLog? logToUpdate = logList.FirstOrDefault(log => log.Id == eventArgs.ExecutedSave.Id);

			if(logToUpdate == null) { return; }

			int index = logList.IndexOf(logToUpdate);

			double progression = Math.Round((1 - (double)(eventArgs.RemainingFiles.Count - 1) / (double)(eventArgs.EligibleFiles.Count - 1)) * 100, 2);
			logList[index] = new FileCopyPreviewLog(id : eventArgs.ExecutedSave.Id,
															name: eventArgs.ExecutedSave.Name,
															sourceFilePath: eventArgs.CurrentFileSourcePath,
															targetFilePath: eventArgs.CurrentFileDestinationPath,
															state: eventArgs.State,
															totalFileToCopy: eventArgs.EligibleFiles.Count,
															totalFileSize: GetTotalFileSize(eventArgs.EligibleFiles),
															nbFilesLeftToDo: eventArgs.RemainingFiles.Count-1,
															progression: progression.ToString() + "%");

			string jsonToLog = SerializeObjectToJson(logList);
			string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

            //Log with logger lib
			LogLibLogger.WriteLog(jsonToLog, pathToWriteTo);

			Thread.Sleep(500);
        }

		private long GetTotalFileSize(List<string> eligibleFiles)
		{
            // Get the total size of the files to copy
            long totalFileSize = 0;
			foreach (string file in eligibleFiles)
			{
				totalFileSize += new FileInfo(file).Length;
			}
			return totalFileSize;
		}

		private string SerializeObjectToJson(object log)
		{
            // write in continue the object to a json file
            var options = new JsonSerializerOptions { WriteIndented = true };
			return JsonSerializer.Serialize(log, options);
		}

		public void OnSaveCreated(object sender, Save eventArgs)
		{
            // Change the status json file
            List<FileCopyPreviewLog> logList = new List<FileCopyPreviewLog>();

			foreach(Save save in Creator.GetSaveStoreInstance().GetAllSaves())
			{
				logList.Add(new FileCopyPreviewLog(id: save.Id,
													name: save.Name,
													sourceFilePath: "",
													targetFilePath: "",
													state: "INACTIVE",
													totalFileToCopy: 0,
													totalFileSize: 0,
													nbFilesLeftToDo: 0,
													progression: "0"));
			}

			string jsonToLog = SerializeObjectToJson(logList);
			string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

			//Log to real time
			LogLibLogger.WriteLog(jsonToLog, pathToWriteTo);
		}

		public void OnSaveFinished(object sender, Save save)
		{
            // Change the status json file
            string pathToRealTimeLog = Path.Combine(Creator.GetSettingsInstance().RealTimeLogPath, "realTimeLog.json");
			string jsonString = File.ReadAllText(pathToRealTimeLog);
			List<FileCopyPreviewLog> logList = JsonSerializer.Deserialize<List<FileCopyPreviewLog>>(jsonString);

			FileCopyPreviewLog? logToUpdate = logList.FirstOrDefault(log => log.Id == save.Id);

			if (logToUpdate == null) { return; }

			int index = logList.IndexOf(logToUpdate);

			logList[index] = new FileCopyPreviewLog(id: save.Id,
													name: save.Name,
													sourceFilePath: "",
													targetFilePath: "",
													state: "INACTIVE",
													totalFileToCopy: 0,
													totalFileSize: 0,
													nbFilesLeftToDo: 0,
													progression: "0");

			string jsonToLog = SerializeObjectToJson(logList);
			string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

			//Log to real time
			LogLibLogger.WriteLog(jsonToLog, pathToWriteTo);
		}
	}
}
