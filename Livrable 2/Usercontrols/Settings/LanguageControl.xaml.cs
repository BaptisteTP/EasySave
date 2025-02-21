using EasySave2._0.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySave2._0.Usercontrols
{
	public partial class LanguageControl : UserControl
	{
		public event EventHandler<LanguageItem>? LanguageChanged;

		public LanguageItem SelectedLanguage
		{
			get { return (LanguageItem)GetValue(SelectedLanguageProperty); }
			set { SetValue(SelectedLanguageProperty, value); }
		}

		public static readonly DependencyProperty SelectedLanguageProperty =
			DependencyProperty.Register("SelectedLanguage", typeof(LanguageItem), typeof(LanguageControl), new PropertyMetadata(null));


		public ObservableCollection<LanguageItem> ItemsSource
		{
			get { return (ObservableCollection<LanguageItem>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<LanguageItem>), typeof(LanguageControl), new PropertyMetadata(new ObservableCollection<LanguageItem>()));

		public LanguageControl()
		{
			InitializeComponent();
		}

		private void Language_Changed(object sender, SelectionChangedEventArgs e)
		{
			LanguageChanged?.Invoke(this, (LanguageItem)e.AddedItems[0]);
		}
	}
}
