using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LogLib
{
    // Class that stores the log of the file copy
    public class Logger
    {
        // Method to write the log of the file copy
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

        // Method to write the log of the file copy
        public void WriteLog(string logMessage, string logDirectory)
        {
			string saveLogFile = Path.Combine(logDirectory, "realTimeLog.json");

            File.WriteAllText(saveLogFile, logMessage);
		}
	}
}
