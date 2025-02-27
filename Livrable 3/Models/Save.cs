using EasySave2._0.Enums;
using EasySave2._0.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace EasySave2._0.ViewModels
{
    public class Save : ModelBase
    {
        // Create a save attribute for other classes to use.
        public int Id { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private string sourcePath;
        public string SourcePath
        {
            get { return sourcePath; }
            set { sourcePath = value; OnPropertyChanged(); }
        }

        private string destinationPath;
        public string DestinationPath
        {
            get { return destinationPath; }
            set { destinationPath = value; OnPropertyChanged(); }
        }

        private bool isExecuting;
		[JsonIgnore]
		[XmlIgnore]
		public bool IsExecuting
        {
            get { return isExecuting; }
            set
            {
                isExecuting = value;
                OnPropertyChanged();
            }
        }

        private int progress;
		public int Progress
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(); }
        }

        private bool encrypt;
        public bool Encrypt
        {
            get { return encrypt; }
            set { encrypt = value; OnPropertyChanged(); }
        }

        private bool isPaused;
		[JsonIgnore]
		[XmlIgnore]
		public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; OnPropertyChanged(); }
        }

        public bool WasSavePausedByUser { get; set; }

		private bool isCopyingCriticalFile;
		[JsonIgnore]
		[XmlIgnore]
		public bool IsCopyingCriticalFile
		{
			get { return isCopyingCriticalFile; }
			set { isCopyingCriticalFile = value; OnPropertyChanged(); }
		}

		private bool isWaitingForCriticalFiles;
		[JsonIgnore]
		[XmlIgnore]
		public bool IsWaitingForCriticalFiles
		{
			get { return isWaitingForCriticalFiles; }
			set { isWaitingForCriticalFiles = value; OnPropertyChanged(); }
		}

		public SaveType Type { get; set; }
        private DateTime? lastExecuteDate = null;

        public DateTime? LastExecuteDate
        {
            get { return lastExecuteDate; }
            set 
            { 
                lastExecuteDate = value;
                OnPropertyChanged(nameof(LastExecutionString));
            }
        }

        public string LastExecutionString => string.Format(Application.Current.Resources["LastSaveExecution"] as string, LastExecuteDate == null ? 
                                                                                                        Application.Current.Resources["NeverExecutedMessage"] as string
                                                                                                        : ((DateTime)LastExecuteDate).ToString("dd/MM/yyyy - HH:mm"));
        private int numberOfExecution = 0;

        public int NumberOfExecution
        {
            get { return numberOfExecution; }
            set 
            { 
                numberOfExecution = value;
				OnPropertyChanged(nameof(NumberOfExecutionString));
			}
		}

        public string NumberOfExecutionString => string.Format(Application.Current.Resources["NumberOfExecutionSave"] as string, NumberOfExecution.ToString());

        private CancellationTokenSource cancellationTokenSource;
        private ManualResetEventSlim pauseEvent;

        public Save(int id, string name, string sourcePath, string destinationPath, SaveType type, bool encrypt)
        {
            Id = id;
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
            Encrypt = encrypt;
            LastExecuteDate = null;
            pauseEvent = new ManualResetEventSlim(true);
        }

        public Save(int id, string name, string sourcePath, string destinationPath, SaveType type, DateTime lastExecutedDate, bool encrypt)
        {
            Id = id;
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
            Encrypt = encrypt;
            LastExecuteDate = lastExecutedDate;
            pauseEvent = new ManualResetEventSlim(true);
        }

        public Save()
        {
            pauseEvent = new ManualResetEventSlim(true);
        }

        public async Task Execute()
        {
            cancellationTokenSource = new CancellationTokenSource();

            Debug.WriteLine("Save execution started for Save ID: " + Id);

            await Task.Run(() =>
            {
                Creator.GetPasterInstance().BeginCopyPaste(this, cancellationTokenSource.Token, pauseEvent);
            }, cancellationTokenSource.Token);

            Debug.WriteLine("Save execution finished for Save ID: " + Id);
        }

        public void Resume()
        {
            IsPaused = false;
            pauseEvent.Set();
        }

        public void Pause()
        {
            pauseEvent.Reset();
            IsPaused = true;
        }

        public void Stop()
        {
            // Cancel the ongoing operation
            cancellationTokenSource?.Cancel();

            pauseEvent.Set();

            // Reset the state properties
            IsExecuting = false;
            IsPaused = false;
            Progress = 0;
        }
    }
}
