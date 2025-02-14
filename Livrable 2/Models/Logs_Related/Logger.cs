using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using EasySave2._0.CustomEventArgs;

namespace EasySave2._0.Models.Logs_Related
{
	public class Logger
	{
		// This class is responsible for logging the events.
		private LogLib.Logger LogLibLogger;
		public Logger()
		{
			LogLibLogger = new LogLib.Logger();
		}

		#region EventHandlers

		public void OnCopyDirectory(object sender, CopyDirectoryEventArgs eventArgs)
		{
			string pathToDailLog = Creator.GetSettingsInstance().DailyLogPath;
			string extension = "." + Creator.GetSettingsInstance().LogFormat;

			//Get the precedings daily logs
			Dailylog daylylog = GetDaylyLogs(pathToDailLog, extension);

			//Create the new directory log object with its attributes
			DirectoryCopyLog log = new DirectoryCopyLog(name: eventArgs.ExecutedSave.Name,
														directoryTransferTime: eventArgs.CreationTimeSpan.ToString() ?? "-1",
														time: eventArgs.CopyDate);

			//Add it to the copied directories list of the current save being made (the added last)
			daylylog.Saves.Last().CopiedDirectories.Add(log);

			//Get the new string for the daily log
			string stringTolog = SerializeLogObject(daylylog);

			//Log with logger lib
			LogLibLogger.OverwrtieDailyLog(stringTolog, pathToDailLog, extension: "." + Creator.GetSettingsInstance().LogFormat);
		}


		public void OnCopyFile(object sender, FileCopyEventArgs eventArgs)
		{
			string pathToDailLog = Creator.GetSettingsInstance().DailyLogPath;
			string extension = "." + Creator.GetSettingsInstance().LogFormat;

			//Get the precedings daily logs
			Dailylog daylyLog = GetDaylyLogs(pathToDailLog, extension);

			//Create the new file log object with its attributes
			FileCopyLog log = new FileCopyLog(name: eventArgs.ExecutedSave.Name,
											  fileSource: eventArgs.SourceFile.FullName,
											  fileTarget: eventArgs.DestinationPath,
											  fileSize: eventArgs.FileSize,
											  fileTransferTime: eventArgs.TransferTime.ToString() ?? "-1",
											  time: eventArgs.CopyDate);

			//Add it to the copied files list of the current save being made (the added last)
			daylyLog.Saves.Last().CopiedFiles.Add(log);

			//Get the new string for the daily log
			string stringToLog = SerializeLogObject(daylyLog);

			//Log with logger lib
			LogLibLogger.OverwrtieDailyLog(stringToLog, pathToDailLog, extension);
		}

		public void OnCopyFilePreview(object sender, FileCopyPreviewEventArgs eventArgs)
		{
			// Write on json file for time log
			string pathToRealTimeLog = Path.Combine(Creator.GetSettingsInstance().RealTimeLogPath, "realTimeLog.json");
			string logString = File.ReadAllText(pathToRealTimeLog);

			List<FileCopyPreviewLog> logList = DeserializeLog(logString);

			FileCopyPreviewLog? logToUpdate = logList.FirstOrDefault(log => log.Id == eventArgs.ExecutedSave.Id);

			if (logToUpdate == null) { return; }

			int index = logList.IndexOf(logToUpdate);

			double progression = Math.Round((1 - (double)(eventArgs.RemainingFiles.Count - 1) / (double)(eventArgs.EligibleFiles.Count - 1)) * 100, 2);
			logList[index] = new FileCopyPreviewLog(id: eventArgs.ExecutedSave.Id,
															name: eventArgs.ExecutedSave.Name,
															sourceFilePath: eventArgs.CurrentFileSourcePath,
															targetFilePath: eventArgs.CurrentFileDestinationPath,
															state: eventArgs.State,
															totalFileToCopy: eventArgs.EligibleFiles.Count,
															totalFileSize: GetTotalFileSize(eventArgs.EligibleFiles),
															nbFilesLeftToDo: eventArgs.RemainingFiles.Count - 1,
															progression: progression.ToString() + "%");

			string stringToLog = SerializeLogObject(logList);
			string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

			//Log with logger lib
			LogLibLogger.WriteLog(stringToLog, pathToWriteTo);

			Thread.Sleep(500);
		}

		public void OnSaveCreated(object sender, EventArgs eventArgs)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);

			// Change the status json file
			List<FileCopyPreviewLog> logList = new List<FileCopyPreviewLog>();
			foreach (Save save in saves)
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

			string stringToLog = SerializeLogObject(logList);
			string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

			//Log to real time
			LogLibLogger.WriteLog(stringToLog, pathToWriteTo);
		}

		private void WriteSavesInLog(List<Save> saves)
		{
			string saveLog = SerializeLogObject(saves);
			LogLibLogger.OverwriteLog(saveLog, "saves." + Creator.GetSettingsInstance().LogFormat, "saves/");
		}

		public void OnSaveDeleted(object sender, EventArgs eventargs)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);
		}

		public void OnSaveEdited(object sender, EventArgs eventargs)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);
		}

		public void OnSaveStarted(object sender, Save save)
		{
			Settings settings = Creator.GetSettingsInstance();
			string pathToDailLog = settings.DailyLogPath;
			string extension = "." + settings.LogFormat;
			Dailylog daylyLog = DeserializeDailyLogs(Path.Combine(pathToDailLog, "save-log", $"{DateTime.Now:yyyy-MM-dd}" + extension));

			daylyLog.Saves.Add(new SaveLog(save.Name));

			string stringToLog = SerializeLogObject(daylyLog);
			string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

			//Log with logger lib
			LogLibLogger.OverwrtieDailyLog(stringToLog, pathToWriteTo, extension);
		}

		public void OnSaveFinished(object sender, Save save)
		{
			Settings settings = Creator.GetSettingsInstance();
			string pathToWriteTo = settings.RealTimeLogPath;

			// Change the status json file
			string pathToRealTimeLog = Path.Combine(settings.RealTimeLogPath, "realTimeLog.json");
			string logString = File.ReadAllText(pathToRealTimeLog);
			List<FileCopyPreviewLog> logList = DeserializeLog(logString);

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

			string jsonToLog = SerializeLogObject(logList);
			//Log to real time
			LogLibLogger.WriteLog(jsonToLog, pathToWriteTo);
		}

		#endregion

		#region Serializing / Deserializing 

		private string SerializeLogObject(object log)
		{
			string? logFormat = Creator.GetSettingsInstance().LogFormat;

			switch (logFormat)
			{
				case "xml":
					XmlWriterSettings xmlSetting = new XmlWriterSettings()
					{
						Indent = true,

					};

					using (var sw = new StringWriter())
					{
						using (XmlWriter writer = XmlWriter.Create(sw, xmlSetting))
						{
							if (log is Dailylog)
							{
								XmlSerializer xsSubmit = new XmlSerializer(typeof(Dailylog));
								xsSubmit.Serialize(writer, log);
							}
							else if (log is List<Save>)
							{
								XmlSerializer xsSubmit = new XmlSerializer(typeof(List<Save>));
								xsSubmit.Serialize(writer, log);
							}
							else if (log is List<FileCopyPreviewLog>)
							{
								XmlSerializer xsSubmit = new XmlSerializer(typeof(List<FileCopyPreviewLog>));
								xsSubmit.Serialize(writer, log);
							}
						}

						return sw.ToString();
					}

				case "json":
					return JsonSerializer.Serialize(log, new JsonSerializerOptions { WriteIndented = true });

				default:
					return JsonSerializer.Serialize(log, new JsonSerializerOptions { WriteIndented = true });
			}

		}

		private Dailylog DeserializeDailyLogs(string pathToDailyLog)
		{
			string logFormat = Creator.GetSettingsInstance().LogFormat;
			if (!File.Exists(pathToDailyLog)) { return new Dailylog(); }

			try{
				switch (logFormat)
				{
					case "xml":
						var mySerializer = new XmlSerializer(typeof(Dailylog));
						//string xml = File.ReadAllText(pathToDailyLog);
						using (TextReader reader = new StreamReader(pathToDailyLog))
						{
							return (Dailylog)mySerializer.Deserialize(reader)!;
						}

					case "json":
						using (FileStream reader = File.OpenRead(pathToDailyLog))
						{
							if (reader.Length == 0) { return new Dailylog(); }
							return JsonSerializer.Deserialize<Dailylog>(reader)!;
						}

					default:
						return new Dailylog();
				}
			}
			catch
			{
				return new Dailylog();
			}
		}

		private List<FileCopyPreviewLog> DeserializeLog(string logString)
		{
			string? logFormat = Creator.GetSettingsInstance().LogFormat;

			switch (logFormat)
			{
				case "xml":
					var mySerializer = new XmlSerializer(typeof(List<FileCopyPreviewLog>));
					using (TextReader reader = new StringReader(logString))
					{
						return (List<FileCopyPreviewLog>)mySerializer.Deserialize(reader)!;
					}

				case "json":
					return JsonSerializer.Deserialize<List<FileCopyPreviewLog>>(logString)!;

				default:
					return null;
			}
		}

		#endregion

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
		private Dailylog GetDaylyLogs(string pathToDailLog, string extension)
		{
			return DeserializeDailyLogs(Path.Combine(pathToDailLog, "save-log", $"{DateTime.Now:yyyy-MM-dd}" + extension));
		}
	}
}
