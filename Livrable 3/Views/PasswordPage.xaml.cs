using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
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

namespace EasySave2._0
{
    /// <summary>
    /// Logique d'interaction pour PasswordPage.xaml
    /// </summary>
    public partial class PasswordPage : Page
    {
        private const string CorrectPassword = "1234";

        public PasswordPage()
        {
            InitializeComponent();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = string.Empty;
        }

        private void ValidatePassword(object sender, RoutedEventArgs e)
        {
            if (mdp.Password == CorrectPassword)
            {
                NavigationService.Navigate(new EncryptPage());
            }
            else
            {
                ErrorMessage.Text = (string)FindResource("IncorrectPassword");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Creator.GetHomePageInstance());
        }

        private void OnPasswordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidatePassword(sender, e);
            }
        }

    }
}
