using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LogLib
{
    // Class that stores the log of the file copy
    public class Logger
    {
        /// <summary>
        /// This function overwrites the content of the file of the current if it exists or it creates a new file and write the content.
        /// </summary>
        /// <param name="logMessage">The information you want to log</param>
        /// <param name="logDirectory">The path to the directory you want to log to</param>
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

        /// <summary>
        /// This function overwrites the content of the realTimeLog.json at the specified path.
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="logDirectory"></param>
        public void WriteLog(string logMessage, string logDirectory)
        {
			string saveLogFile = Path.Combine(logDirectory, "realTimeLog.json");

            File.WriteAllText(saveLogFile, logMessage);
		}
	}
}
