using EasySave2._0.ViewModels;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System.Xml;
using EasySave2._0.CustomEventArgs;

namespace EasySave2._0.Models.Logs_Related
{
	public class Logger
	{
		private object _dailyLogLock = new object();
		private object _RealTimeLock = new object();

		// This class is responsible for logging the events.
		private LogLib.Logger LogLibLogger;
		public Logger()
		{
			LogLibLogger = new LogLib.Logger();
		}

		#region Save Execution Handlers

		/// <summary>
		/// Handle the copied diretory event.
		/// Writes the information of the copied directory in the daily logs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void OnCopyDirectory(object sender, CopyDirectoryEventArgs eventArgs)
		{
			lock (_dailyLogLock)
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
				daylylog.Saves.Last(savelog => savelog.Id == eventArgs.ExecutedSave.Id).CopiedDirectories.Add(log);

				//Get the new string for the daily log
				string stringTolog = SerializeLogObject(daylylog);

				//Log with logger lib
				WriteToDailyLog(stringTolog, pathToDailLog,"." + Creator.GetSettingsInstance().LogFormat);
			}
		}

		/// <summary>
		/// Handle the copied file event.
		/// Write the information of the copied file in the daily logs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void OnCopyFile(object sender, FileCopyEventArgs eventArgs)
		{
			lock (_dailyLogLock)
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
				daylyLog.Saves.Last(savelog => savelog.Id == eventArgs.ExecutedSave.Id).CopiedFiles.Add(log);

				//Get the new string for the daily log
				string stringToLog = SerializeLogObject(daylyLog);

				//Log with logger lib
				WriteToDailyLog(stringToLog, pathToDailLog, extension);
			}
		}

		/// <summary>
		/// Handle the copy preview event.
		/// Write all the preview information in the real time log.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void OnCopyFilePreview(object sender, FileCopyPreviewEventArgs eventArgs)
		{
			lock (_RealTimeLock)
			{
				// Write on json file for time log
				string pathToRealTimeLog = Path.Combine(Creator.GetSettingsInstance().RealTimeLogPath, "realTimeLog.json");
				List<FileCopyPreviewLog> logList = DeserializeLog(pathToRealTimeLog);

				FileCopyPreviewLog? logToUpdate = logList.FirstOrDefault(log => log.Id == eventArgs.ExecutedSave.Id);

				if (logToUpdate == null) { return; }

				int index = logList.IndexOf(logToUpdate);
				logList[index] = new FileCopyPreviewLog(id: eventArgs.ExecutedSave.Id,
																name: eventArgs.ExecutedSave.Name,
																sourceFilePath: eventArgs.CurrentFileSourcePath,
																targetFilePath: eventArgs.CurrentFileDestinationPath,
																state: eventArgs.State,
																totalFileToCopy: eventArgs.EligibleFiles.Count,
																totalFileSize: GetTotalFileSize(eventArgs.EligibleFiles),
																nbFilesLeftToDo: eventArgs.RemainingFiles.Count - 1,
																progression: eventArgs.ExecutedSave.Progress.ToString() + "%");

				string stringToLog = SerializeLogObject(logList);
				string pathToWriteTo = Creator.GetSettingsInstance().RealTimeLogPath;

				//Log with logger lib
				WriteLog(stringToLog, pathToWriteTo);

				//Thread.Sleep(300);
			}
		}

		/// <summary>
		/// Handle the save started event.
		/// Create the started save log in the daily log.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="save"></param>
		public void OnSaveStarted(object sender, Save save)
		{
			lock (_dailyLogLock)
			{
				Settings settings = Creator.GetSettingsInstance();
				string pathToDailLog = settings.DailyLogPath;
				string extension = "." + settings.LogFormat;

				Dailylog daylyLog = DeserializeDailyLogs(Path.Combine(pathToDailLog, "save-log", $"{DateTime.Now:yyyy-MM-dd}" + extension));

				daylyLog.Saves.Add(new SaveLog(save.Id, save.Name));

				string stringToLog = SerializeLogObject(daylyLog);
				string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

				//Log with logger lib
				WriteToDailyLog(stringToLog, pathToWriteTo, extension);
			}
		}

		/// <summary>
		/// Handle the save finished event.
		/// Swap the state of the finished save to 'INACTIVE' in the real time log.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="save"></param>
		public void OnSaveFinished(object sender, Save save)
		{
			lock (_RealTimeLock)
			{
				Settings settings = Creator.GetSettingsInstance();
				string pathToWriteTo = settings.RealTimeLogPath;

				// Change the status json file
				string pathToRealTimeLog = Path.Combine(settings.RealTimeLogPath, "realTimeLog.json");
				List<FileCopyPreviewLog> logList = DeserializeLog(pathToRealTimeLog);

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
				WriteLog(jsonToLog, pathToWriteTo);
			}
		}

		public void OnBuisnessSoftwareDetected(object sender, Save save)
		{
			lock (_dailyLogLock)
			{
				Settings settings = Creator.GetSettingsInstance();
				string pathToDailLog = settings.DailyLogPath;
				string extension = "." + settings.LogFormat;

				Dailylog daylyLog = DeserializeDailyLogs(Path.Combine(pathToDailLog, "save-log", $"{DateTime.Now:yyyy-MM-dd}" + extension));

				SaveLog savelog = new SaveLog(save.Id, save.Name);
				savelog.State = "Buisness Software Detected";
				daylyLog.Saves.Add(savelog);

				string stringToLog = SerializeLogObject(daylyLog);
				string pathToWriteTo = Creator.GetSettingsInstance().DailyLogPath;

				//Log with logger lib
				WriteToDailyLog(stringToLog, pathToWriteTo, extension);
			}
		}

		#endregion

		#region Save CRUD Handlers
		public void OnSavesLoaded(object sender, EventArgs eventArgs)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);

			lock (_RealTimeLock)
			{
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
				WriteLog(stringToLog, pathToWriteTo);
			}
		}

		public void OnSaveCreated(object sender, Save saveCreated)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);

			lock (_RealTimeLock)
			{
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
				WriteLog(stringToLog, pathToWriteTo);
			}
		}
		public void OnSaveEdited(object sender, Save savedEdited)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);
		}
		public void OnSaveDeleted(object sender, Save saveDeleted)
		{
			List<Save> saves = Creator.GetSaveStoreInstance().GetAllSaves();
			WriteSavesInLog(saves);
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

		private List<FileCopyPreviewLog> DeserializeLog(string pathToLog)
		{
			string? logFormat = Creator.GetSettingsInstance().LogFormat;
			string logString = File.ReadAllText(pathToLog);
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

		#region Utilities

		private void WriteSavesInLog(List<Save> saves)
		{
			string saveLog = SerializeLogObject(saves);
			LogLibLogger.OverwriteLog(saveLog, "saves." + Creator.GetSettingsInstance().LogFormat, "saves/");
		}

		private void WriteLog(string stringToLog, string pathToWriteTo)
		{
			LogLibLogger.WriteLog(stringToLog, pathToWriteTo);
		}

		private void WriteToDailyLog(string stringToLog, string pathToDailLog, string extension)
		{
			LogLibLogger.OverwrtieDailyLog(stringToLog, pathToDailLog, extension);
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
		private Dailylog GetDaylyLogs(string pathToDailLog, string extension)
		{
			return DeserializeDailyLogs(Path.Combine(pathToDailLog, "save-log", $"{DateTime.Now:yyyy-MM-dd}" + extension));
		}

		#endregion
	}
}
