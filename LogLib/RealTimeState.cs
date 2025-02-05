using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace LogLib
{
    public class RealTimeStateLogger
    {
        private static readonly string stateFilePath;

        static RealTimeStateLogger()
        {
            string logDirectory = Path.Combine(@"C:\temp", "logs-save");
            stateFilePath = Path.Combine(logDirectory, "state.json");

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du répertoire des logs : {ex.Message}");
            }
        }

        public class StateEntry
        {
            public string Name { get; set; }
            public string SourceFilePath { get; set; }
            public string TargetFilePath { get; set; }
            public string State { get; set; }
            public int TotalFilesToCopy { get; set; }
            public long TotalFilesSize { get; set; }
            public int NbFilesLeftToDo { get; set; }
            public int Progression { get; set; }

            public static void UpdateState(string name, string sourceFilePath, string targetFilePath, string state, int totalFiles, long totalSize, int remainingFiles)
            {
                List<StateEntry> stateList;

                try
                {
                    if (File.Exists(stateFilePath))
                    {
                        string existingContent = File.ReadAllText(stateFilePath);
                        stateList = JsonSerializer.Deserialize<List<StateEntry>>(existingContent) ?? new List<StateEntry>();
                    }
                    else
                    {
                        stateList = new List<StateEntry>();
                    }

                    var existingState = stateList.Find(s => s.Name == name);
                    if (existingState != null)
                    {
                        existingState.SourceFilePath = sourceFilePath;
                        existingState.TargetFilePath = targetFilePath;
                        existingState.State = state;
                        existingState.TotalFilesToCopy = totalFiles;
                        existingState.TotalFilesSize = totalSize;
                        existingState.NbFilesLeftToDo = remainingFiles;
                        existingState.Progression = totalFiles > 0 ? 100 * (totalFiles - remainingFiles) / totalFiles : 0;
                    }
                    else
                    {
                        stateList.Add(new StateEntry
                        {
                            Name = name,
                            SourceFilePath = sourceFilePath,
                            TargetFilePath = targetFilePath,
                            State = state,
                            TotalFilesToCopy = totalFiles,
                            TotalFilesSize = totalSize,
                            NbFilesLeftToDo = remainingFiles,
                            Progression = totalFiles > 0 ? 100 * (totalFiles - remainingFiles) / totalFiles : 0
                        });
                    }

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    File.WriteAllText(stateFilePath, JsonSerializer.Serialize(stateList, options));

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la mise à jour de l'état en temps réel : {ex.Message}");
                }
            }
        }
    }
}