using EasySave2._0.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
	public class WelcomeViewModel : ViewModelBase
	{
		public event EventHandler? NextPageButtonClicked;
		public ICommand GoNext { get; }

        public ObservableCollection<LanguageItem> LanguageItems { get; } = new ObservableCollection<LanguageItem>(Creator.GetAvalaibleLanguages());
		public LanguageItem SelectedLanguage { get; private set; } 

		public WelcomeViewModel()
		{
			GoNext = new RelayCommand(GoToNextPage, CanGoNext);
			SelectedLanguage = LanguageItems.FirstOrDefault(lang => lang.Language.Equals("en-US"));
		}

		private void GoToNextPage(object obj)
		{
			NextPageButtonClicked?.Invoke(this, EventArgs.Empty);
		}

		private bool CanGoNext(object arg)
		{
			return !string.IsNullOrEmpty(SelectedLanguage.Language);
		}

        public void LanguageControl_LanguageChanged(object? sender, LanguageItem e)
        {
            Settings.ChangeSetting("ActiveLanguage", e.Language);
            Settings.ApplyLanguageSettings();
        }
    }
}
