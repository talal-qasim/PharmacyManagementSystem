using System.Windows;

namespace PharmacyApp.Views
{
    public partial class UserView : Window
    {
        public UserView()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
