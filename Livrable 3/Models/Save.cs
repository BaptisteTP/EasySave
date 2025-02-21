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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; OnPropertyChanged(); }
        }



        public SaveType Type { get; set; }
        public DateTime? LastExecuteDate { get; set; }

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
            IsExecuting = true;
            Progress = 0;

            Debug.WriteLine("Save execution started for Save ID: " + Id);

            await Task.Run(() =>
            {
                Creator.GetPasterInstance().BeginCopyPaste(this, cancellationTokenSource.Token, pauseEvent);
            }, cancellationTokenSource.Token);

            IsExecuting = false;
            Progress = 0;
            IsPaused = false;

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
            cancellationTokenSource?.Cancel();
            IsExecuting = false;
            Progress = 0;
        }
    }
}
