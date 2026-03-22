using System.Windows.Controls;
using System.Windows.Input;

namespace Tudormobile.IronLedgerLib.UI.Wpf.Views;

/// <summary>
/// Interaction logic for AssetView.xaml
/// </summary>
public partial class AssetView : UserControl
{
    private readonly Dictionary<TextBox, string> _originalValues = [];

    /// <summary>
    /// Initializes a new instance of the AssetView class.
    /// </summary>
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
                // Restore original value before clearing focus so bindings see the reverted text.
                textBox.Text = _originalValues.TryGetValue(textBox, out string? value) ? value : string.Empty;
                _originalValues.Remove(textBox);
                Keyboard.ClearFocus();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                // If empty, treat as cancel by restoring original text before clearing focus.
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = _originalValues.TryGetValue(textBox, out string? value) ? value : string.Empty;
                    _originalValues.Remove(textBox);
                }
                Keyboard.ClearFocus();
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
}
