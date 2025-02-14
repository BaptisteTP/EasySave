using EasySave2._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave2._0.ViewModels
{
	public class WelcomeViewModel : ViewModelBase
	{
		public event EventHandler? NextPageButtonClicked;
		public ICommand SetAppLanguage { get; }
		public ICommand GoNext { get; }
		private string SelectedLanguage { get; set; }

		public WelcomeViewModel()
		{
			SetAppLanguage = new RelayCommand(SetApplicationLanguage, CanExecute);
			GoNext = new RelayCommand(GoToNextPage, CanGoNext);
		}

		private void GoToNextPage(object obj)
		{
			NextPageButtonClicked?.Invoke(this, EventArgs.Empty);
		}

		private bool CanGoNext(object arg)
		{
			return !string.IsNullOrEmpty(SelectedLanguage);
		}


		private void SetApplicationLanguage(object language)
		{
			SelectedLanguage = (string)language;

			Settings.ChangeSetting("ActiveLanguage", SelectedLanguage);
			Settings.ApplyLanguageSettings();
		}
		private bool CanExecute(object arg)
		{
			return true;
		}
	}
}
