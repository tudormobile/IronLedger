using System.Windows.Controls;
using System.Windows.Input;

namespace AssetViewer.Views
{
    /// <summary>
    /// Interaction logic for AssetView.xaml
    /// </summary>
    public partial class AssetView : UserControl
    {
        private readonly Dictionary<TextBox, string> _originalValues = [];
        public AssetView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Escape)
                {

                    Keyboard.ClearFocus();
                    textBox.Text = _originalValues.TryGetValue(textBox, out string? value) ? value : string.Empty;
                    _originalValues.Remove(textBox);
                    e.Handled = true;
                }
                if (e.Key == Key.Enter)
                {
                    Keyboard.ClearFocus();
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = _originalValues.TryGetValue(textBox, out string? value) ? value : string.Empty;
                        _originalValues.Remove(textBox);
                    }
                    e.Handled = true;
                }
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                _originalValues[textBox] = textBox.Text;
            }
        }

        private void TextBlock_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
