using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyMessageBox
{
    /// <summary>
    /// Borderless modal dialog used across the app for confirmations and error/info messages
    /// (per rulebook Rule 8.4 modal-dialog standard). Do not instantiate directly from
    /// ViewModels — call the static CustomMessageBoxView.Show(...) helper instead, which
    /// mirrors how Views.[Entity]View are launched via .ShowDialog().
    /// </summary>
    public partial class CustomMessageBoxView : Window
    {
        public MessageBoxResultType Result { get; private set; } = MessageBoxResultType.None;

        public CustomMessageBoxView(string title, string message, MessageBoxIconType icon, MessageBoxButtonsType buttons)
        {
            InitializeComponent();

            TitleText.Text = title;
            MessageText.Text = message;

            TitleText.Foreground = icon switch
            {
                MessageBoxIconType.Error => new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x35)),
                MessageBoxIconType.Warning => new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)),
                MessageBoxIconType.Success => new SolidColorBrush(Color.FromRgb(0x43, 0xA0, 0x47)),
                _ => TitleText.Foreground
            };

            BuildButtons(buttons);
        }

        private void BuildButtons(MessageBoxButtonsType buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtonsType.Ok:
                    AddButton("OK", MessageBoxResultType.Ok, isDefault: true);
                    break;
                case MessageBoxButtonsType.OkCancel:
                    AddButton("Cancel", MessageBoxResultType.Cancel);
                    AddButton("OK", MessageBoxResultType.Ok, isDefault: true);
                    break;
                case MessageBoxButtonsType.YesNo:
                    AddButton("No", MessageBoxResultType.No);
                    AddButton("Yes", MessageBoxResultType.Yes, isDefault: true);
                    break;
                case MessageBoxButtonsType.YesNoCancel:
                    AddButton("Cancel", MessageBoxResultType.Cancel);
                    AddButton("No", MessageBoxResultType.No);
                    AddButton("Yes", MessageBoxResultType.Yes, isDefault: true);
                    break;
            }
        }

        private void AddButton(string content, MessageBoxResultType result, bool isDefault = false)
        {
            var button = new Button
            {
                Content = content,
                Width = 90,
                Height = 34,
                Margin = new Thickness(8, 0, 0, 0),
                IsDefault = isDefault
            };
            button.Click += (s, e) =>
            {
                Result = result;
                DialogResult = true;
            };
            ButtonsPanel.Children.Add(button);
        }

        /// <summary>
        /// Shows the dialog synchronously and returns the button the user selected.
        /// Usage: MessageBoxResultType r = CustomMessageBoxView.Show("Delete?", "...", MessageBoxIconType.Warning, MessageBoxButtonsType.YesNo);
        /// </summary>
        public static MessageBoxResultType Show(string title, string message, MessageBoxIconType icon, MessageBoxButtonsType buttons)
        {
            var dialog = new CustomMessageBoxView(title, message, icon, buttons);
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}
