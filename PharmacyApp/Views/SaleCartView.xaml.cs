using System.Windows;

namespace PharmacyApp.Views
{
    public partial class SaleCartView : Window
    {
        public SaleCartView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
