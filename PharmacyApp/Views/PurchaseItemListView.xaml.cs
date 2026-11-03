using System.Windows;

namespace PharmacyApp.Views
{
    public partial class PurchaseItemListView : Window
    {
        public PurchaseItemListView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}