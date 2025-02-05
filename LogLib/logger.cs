using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LogLib
{
    public class Logger
    {
        public void WriteDailyLog(object logEntry, string logDirectory)
        {
        
            string saveLogDirectory = Path.Combine(logDirectory, "save-log");

            if (!Directory.Exists(saveLogDirectory))
            {
                Directory.CreateDirectory(saveLogDirectory);
            }

            string logFilePath = Path.Combine(saveLogDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");

            List<object> logList = new List<object>();
            if (File.Exists(logFilePath))
            {
                string existingLogs = File.ReadAllText(logFilePath);
                logList = JsonSerializer.Deserialize<List<object>>(existingLogs) ?? new List<object>();
            }

            logList.Add(logEntry);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(logFilePath, JsonSerializer.Serialize(logList, options));
        }

        public void WriteLog(string logMessage, string logDirectory)
        {
			string saveLogFile = Path.Combine(logDirectory, "realTimeLog.json");

            File.WriteAllText(saveLogFile, logMessage);
		}
	}
}
