using EasySave2._0.Enums;
using EasySave2._0.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave2._0.ViewModels
{
    public class InfoPopupViewModel : INotifyPropertyChanged
    {
        private Save saveToDisplay;
        private SaveStore saveStore = new SaveStore();

        public Save SaveToEdit
        {
            get
            {
                return saveToDisplay;
            }
            set
            {
                saveToDisplay = value;
                SaveName = saveToDisplay.Name;
                SourcePath = saveToDisplay.SourcePath;
                DestinationPath = saveToDisplay.DestinationPath;
                if (Enum.IsDefined(typeof(SaveType), saveToDisplay.Type))
                {
                    SelectedSaveType = saveToDisplay.Type;
                }
                else
                {
                    SelectedSaveType = SaveType.Full;
                }
                Encrypt = saveToDisplay.Encrypt;
                FileCount = saveStore.CountFilesInDirectory(saveToDisplay.SourcePath);
                DirectorySize = saveStore.GetDirectorySize(saveToDisplay.SourcePath);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _encrypt;
        public bool Encrypt
        {
            get { return _encrypt; }
            set
            {
                _encrypt = value;
                OnPropertyChanged();
            }
        }

        private string _saveName;
        public string SaveName
        {
            get { return _saveName; }
            set
            {
                _saveName = value;
                OnPropertyChanged();
            }
        }

        private SaveType selectedSaveType = SaveType.Full;
        public SaveType SelectedSaveType
        {
            get { return selectedSaveType; }
            set
            {
                selectedSaveType = value;
                OnPropertyChanged();
            }
        }

        private string _sourcePath;
        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                OnPropertyChanged();
            }
        }

        private string _destinationPath;
        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                _destinationPath = value;
                OnPropertyChanged();
            }
        }

        private int _fileCount;
        public int FileCount
        {
            get { return _fileCount; }
            set
            {
                _fileCount = value;
                OnPropertyChanged();
            }
        }

        private long _directorySize;
        public long DirectorySize
        {
            get { return _directorySize; }
            set
            {
                _directorySize = value;
                FormattedDirectorySize = FormatSize(_directorySize);
                OnPropertyChanged();
            }
        }

        private string _formattedDirectorySize;
        public string FormattedDirectorySize
        {
            get { return _formattedDirectorySize; }
            set
            {
                _formattedDirectorySize = value;
                OnPropertyChanged();
            }
        }

        private string FormatSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
