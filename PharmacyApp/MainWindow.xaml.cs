using System.Windows;
using System.Windows.Input;

namespace PharmacyApp
{
    /// <summary>
    /// Window control button handlers (close/minimize/restore) for the borderless
    /// custom title bar, per the rulebook's directory-structure comment for MainWindow.xaml.cs.
    /// All actual data/state lives in MainViewModel — this code-behind only touches
    /// window chrome, never the DataContext.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
