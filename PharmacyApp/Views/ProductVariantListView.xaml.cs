using System.Windows;

namespace PharmacyApp.Views
{
    public partial class ProductVariantListView : Window
    {
        public ProductVariantListView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}