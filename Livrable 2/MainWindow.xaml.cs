using System.Windows;
using System.Windows.Navigation;

namespace EasySave2._0
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new WelcomePage());
        }
    }
}
