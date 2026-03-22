using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tudormobile.IronLedgerLib.UI.Wpf.Converters;

/// <summary>
/// Converts a value to a <see cref="Visibility"/> based on whether the value is considered
/// "present" (visible) or "absent" (non-visible).
/// </summary>
/// <remarks>
/// <para>The following rules determine whether a value is visible:</para>
/// <list type="bullet">
///   <item><description><see langword="null"/> → non-visible.</description></item>
///   <item><description>A value type equal to its default (e.g. <c>0</c>, <c>false</c>) → non-visible.</description></item>
///   <item><description>A <see cref="string"/> that is <see langword="null"/>, empty, or whitespace → non-visible.</description></item>
///   <item><description>All other values → visible.</description></item>
/// </list>
/// <para>
/// An optional <see cref="Visibility"/> converter parameter controls output behavior:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Hidden"/> — the non-visible
///       state produces that specific value instead of the default <see cref="Visibility.Collapsed"/>.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="Visibility.Visible"/> — inverts the conversion: non-visible values produce
///       <see cref="Visibility.Visible"/> and visible values produce <see cref="Visibility.Collapsed"/>.
///     </description>
///   </item>
/// </list>
/// </remarks>
public class VisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a value to a <see cref="Visibility"/>.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">
    /// An optional <see cref="Visibility"/> that controls non-visible state or inverts the result.
    /// </param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>A <see cref="Visibility"/> value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isVisible = IsVisible(value);

        var nonVisibleState = Visibility.Collapsed;
        var invert = false;

        if (parameter is Visibility visibilityParameter)
        {
            if (visibilityParameter == Visibility.Visible)
            {
                invert = true;
            }
            else
            {
                nonVisibleState = visibilityParameter;
            }
        }

        if (invert)
        {
            return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        return isVisible ? Visibility.Visible : nonVisibleState;
    }

    /// <summary>
    /// Not supported. Always throws <see cref="NotImplementedException"/>.
    /// </summary>
    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static bool IsVisible(object? value)
    {
        if (value is null)
        {
            return false;
        }

        if (value is string stringValue)
        {
            return !string.IsNullOrWhiteSpace(stringValue);
        }

        if (value is ValueType)
        {
            var defaultValue = Activator.CreateInstance(value.GetType());
            return !value.Equals(defaultValue);
        }

        return true;
    }
}
