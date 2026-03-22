using System.Windows;
using Tudormobile.IronLedgerLib.UI.Wpf.Converters;

namespace IronLedgerLib.UI.Wpf.Tests.Converters;

[TestClass]
public class VisibilityConverterTests
{
    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenValueIsTrue()
    {
        // Arrange
        var converter = new VisibilityConverter();
        var value = true;

        // Act
        var result = converter.Convert(value, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenValueIsFalse()
    {
        // Arrange
        var converter = new VisibilityConverter();
        var value = false;

        // Act
        var result = converter.Convert(value, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(Visibility.Collapsed, result);
    }

    // --- Null / reference types ---

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenValueIsNull()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(null, null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenValueIsNonNullObject()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(new object(), null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Visible, result);
    }

    // --- Strings ---

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenValueIsEmptyString()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(string.Empty, null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenValueIsWhitespaceString()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert("   ", null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenValueIsNonEmptyString()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert("hello", null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Visible, result);
    }

    // --- Value type defaults ---

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenValueIsDefaultInt()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(0, null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenValueIsNonDefaultInt()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(42, null, null, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Visible, result);
    }

    // --- Parameter = Visibility.Collapsed (explicit non-visible state) ---

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenParameterIsCollapsedAndValueIsNull()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(null, null, Visibility.Collapsed, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    // --- Parameter = Visibility.Hidden (alternate non-visible state) ---

    [TestMethod]
    public void Convert_ShouldReturnHidden_WhenParameterIsHiddenAndValueIsNull()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(null, null, Visibility.Hidden, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Hidden, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnHidden_WhenParameterIsHiddenAndValueIsDefaultInt()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(0, null, Visibility.Hidden, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Hidden, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenParameterIsHiddenAndValueIsNonNull()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert("hello", null, Visibility.Hidden, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Visible, result);
    }

    // --- Parameter = Visibility.Visible (inverted behavior) ---

    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenParameterIsVisibleAndValueIsNull()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(null, null, Visibility.Visible, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnVisible_WhenParameterIsVisibleAndValueIsFalse()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(false, null, Visibility.Visible, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenParameterIsVisibleAndValueIsNonNull()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(new object(), null, Visibility.Visible, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void Convert_ShouldReturnCollapsed_WhenParameterIsVisibleAndValueIsTrue()
    {
        var converter = new VisibilityConverter();

        var result = converter.Convert(true, null, Visibility.Visible, CultureInfo.InvariantCulture);

        Assert.AreEqual(Visibility.Collapsed, result);
    }

    // --- ConvertBack ---

    [TestMethod]
    public void ConvertBack_ShouldThrowNotImplementedException()
    {
        var converter = new VisibilityConverter();

        Assert.ThrowsExactly<NotImplementedException>(
            () => converter.ConvertBack(Visibility.Visible, null, null, CultureInfo.InvariantCulture));
    }
}