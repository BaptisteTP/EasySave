using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LogLib
{
    public class Logger
    {
        public void WriteDailyLog(string logMessage, string logDirectory)
        {
        
            string saveLogDirectory = Path.Combine(logDirectory, "save-log");

            if (!Directory.Exists(saveLogDirectory))
            {
                Directory.CreateDirectory(saveLogDirectory);
            }
            
            string logFilePath = Path.Combine(saveLogDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");

            File.AppendAllText(logFilePath, logMessage);
        }

        public void WriteLog(string logMessage, string logDirectory)
        {
			string saveLogFile = Path.Combine(logDirectory, "realTimeLog.json");

            File.WriteAllText(saveLogFile, logMessage);
		}
	}
}
