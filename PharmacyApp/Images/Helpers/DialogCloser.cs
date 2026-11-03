using System.Windows;

namespace PharmacyApp.Images.Helpers
{
    /// <summary>
    /// Attached property used in every modal View's root Window tag:
    ///   med:DialogCloser.DialogResult="{Binding CloseDialog}"
    /// When the bound ViewModel's CloseDialog (bool?) is set from a command
    /// (e.g. Save/Cancel), this closes the Window automatically (per rulebook Rule 8.4).
    /// </summary>
    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }

        public static bool? GetDialogResult(Window target)
        {
            return (bool?)target.GetValue(DialogResultProperty);
        }
    }
}
