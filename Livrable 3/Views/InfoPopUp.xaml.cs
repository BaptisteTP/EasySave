using EasySave2._0.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EasySave2._0
{
    public partial class InfoPopup : UserControl
    {
        private Save saveToDisplay;
        public Save SaveToDisplay
        {
            get { return saveToDisplay; }
            set
            {
                saveToDisplay = value;
                InfoPopupViewModel viewModel = new InfoPopupViewModel();
                viewModel.SaveToEdit = saveToDisplay;
                DataContext = viewModel;
            }
        }
        public InfoPopup()
        {
            InitializeComponent();
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            ((Window)this.Parent).Close(); // Ferme la popup
        }
    }
}
