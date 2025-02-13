using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.Models
{
	public class ValidationModelBase : ModelBase, IDataErrorInfo
	{
		public delegate string ValidateProperty(string propertyName);

		private bool _canValidate;
		public bool CanValidate
		{
			get { return _canValidate; }
			set { _canValidate = value; }
		}

		#region IDataErrorInfo Members

		public string Error
		{
			get { return string.Empty; }
		}

		public string this[string columnName]
		{
			get
			{
				if (this.CanValidate)
				{
					return this.Validate(columnName);
				}
				return string.Empty;
			}
		}

		#endregion

		#region Validation Section

		public event ValidateProperty? OnValidateProperty;

		public string Validate([CallerMemberName] string propertyName = "")
		{
			if (this.OnValidateProperty != null)
			{
				return OnValidateProperty(propertyName);
			}
			return string.Empty;
		}

		#endregion
	}
}
