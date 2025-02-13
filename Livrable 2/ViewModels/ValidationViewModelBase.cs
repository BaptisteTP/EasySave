using EasySave2._0.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.Models
{
	public class ValidationViewModelBase : ViewModelBase, INotifyDataErrorInfo
	{
		protected readonly Dictionary<string, List<string>> _propertyErrors = new Dictionary<string, List<string>>();	
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
		public bool HasErrors => _propertyErrors.Any();

		public IEnumerable GetErrors(string? propertyName)
		{
			return _propertyErrors.GetValueOrDefault(propertyName, null);
		}

		protected void AddError(string propertyName, string errorMessage)
		{
			if (!_propertyErrors.ContainsKey(propertyName))
			{
				_propertyErrors.Add(propertyName, new List<string>());
			}

			_propertyErrors[propertyName].Add(errorMessage);
			OnErrorsChanged(propertyName);

		}

		protected void ClearError(string propertyName)
		{
			if (_propertyErrors.Remove(propertyName))
			{
				OnErrorsChanged(propertyName);
			}

		}

		protected void OnErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}
	}
}
