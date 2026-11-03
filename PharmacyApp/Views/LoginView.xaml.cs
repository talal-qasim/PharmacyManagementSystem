using System.Windows;
using PharmacyApp.ViewModels;

namespace PharmacyApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            this.Loaded += LoginView_Loaded;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm && !string.IsNullOrEmpty(vm.Password))
            {
                txtPassword.Password = vm.Password;
                txtVisiblePassword.Text = vm.Password;
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtVisiblePassword.Visibility != Visibility.Visible)
            {
                txtVisiblePassword.Text = txtPassword.Password;
            }
            UpdateViewModel(txtPassword.Password);
        }

        private void txtVisiblePassword_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtVisiblePassword.Visibility == Visibility.Visible)
            {
                txtPassword.Password = txtVisiblePassword.Text;
            }
            UpdateViewModel(txtVisiblePassword.Text);
        }

        private void UpdateViewModel(string password)
        {
            if (this.DataContext is LoginViewModel vm)
            {
                vm.Password = password;
            }
        }

        private void btnShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPassword.Visibility = Visibility.Collapsed;
            txtVisiblePassword.Visibility = Visibility.Visible;
            txtVisiblePassword.Focus();
        }

        private void btnShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtVisiblePassword.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Focus();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
