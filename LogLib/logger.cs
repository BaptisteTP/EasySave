using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace LogLib
{
    public class Logger
    {
        private static readonly string logDirectory;
        private static readonly string logFile;
        private static readonly string logPath;

        static Logger()
        {
            logDirectory = Path.Combine(@"C:\temp", "logs-save");
            logFile = $"{DateTime.Now:yyyy-MM-dd}.json";    
            logPath = Path.Combine(logDirectory, logFile);

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la creation du repertoire des logs : {ex.Message}");
            }
        }

        public class LogEntry
        {
            public string Name { get; set; }
            public string FileSource { get; set; }
            public string FileTarget { get; set; }
            public long FileSize { get; set; }
            public double FileTransferTime { get; set; }
            public string Time { get; set; }
        }

        public static void LogAction(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime)
        {
            var logEntry = new LogEntry
            {
                Name = name,
                FileSource = fileSource,
                FileTarget = fileTarget,
                FileSize = fileSize,
                FileTransferTime = fileTransferTime,
                Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            List<LogEntry> logList;

            try
            {
                if (File.Exists(logPath))
                {
                    string existingContent = File.ReadAllText(logPath);
                    logList = JsonSerializer.Deserialize<List<LogEntry>>(existingContent) ?? new List<LogEntry>();
                }
                else
                {
                    logList = new List<LogEntry>();
                }

                logList.Add(logEntry);

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(logPath, JsonSerializer.Serialize(logList, options));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'enregistrement de l'action : {ex.Message}");
            }
        }
    }
}