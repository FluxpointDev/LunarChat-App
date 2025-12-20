using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides attached properties for enhancing TextBox functionality.
/// </summary>
/// <remarks>
///     The TextBoxAssist class offers attached properties that can be used to extend the functionality
///     of standard TextBox controls in Avalonia applications. Currently, it provides number formatting
///     capabilities that automatically format numeric input according to specified format strings when
///     the TextBox loses focus.
///     Example usage in XAML:
///     <code>
/// &lt;TextBox Text="{Binding Amount}" 
///          extensions:TextBoxAssist.FormatNumber="C2" /&gt;
/// </code>
///     This will format the number as currency with 2 decimal places when the TextBox loses focus.
/// </remarks>
public class TextBoxAssist
{
    static TextBoxAssist()
    {
        FormatNumberProperty.Changed.AddClassHandler<TextBox>(HandleFormatNumberChanged);
    }

    /// <summary>
    ///     Gets or sets the number format string to be applied to a TextBox when it loses focus.
    /// </summary>
    /// <remarks>
    ///     This property allows automatic formatting of numeric input according to standard .NET format strings.
    ///     The formatting is applied when the TextBox loses focus.
    ///     Supports standard numeric format strings including currency (C), percentage (P), etc.
    /// </remarks>
    public static readonly AttachedProperty<string> FormatNumberProperty =
        AvaloniaProperty.RegisterAttached<TextBoxAssist, TextBox, string>(
            "FormatNumber", string.Empty, false, BindingMode.OneTime);

    /// <summary>
    ///     Sets the number format string to be applied to the specified TextBox when it loses focus.
    /// </summary>
    /// <param name="element">The TextBox element to set the format on.</param>
    /// <param name="value">The format string to apply (e.g., "C2", "P0", "N1").</param>
    public static void SetFormatNumber(AvaloniaObject element, string value)
    {
        element.SetValue(FormatNumberProperty, value);
    }

    /// <summary>
    ///     Gets the number format string applied to the specified TextBox.
    /// </summary>
    /// <param name="element">The TextBox element to get the format from.</param>
    /// <returns>The format string applied to the TextBox.</returns>
    public static string GetFormatNumber(AvaloniaObject element)
    {
        return element.GetValue(FormatNumberProperty);
    }

    private static void HandleFormatNumberChanged(AvaloniaObject element, AvaloniaPropertyChangedEventArgs args)
    {
        if (element is not TextBox textBox) return;

        textBox.LostFocus += (_, _) =>
        {
            var format = GetFormatNumber(textBox);
            if (string.IsNullOrEmpty(format)) return;

            var text = textBox.Text;

            if (string.IsNullOrEmpty(text)) return;

            // Remove currency symbols, percentage signs, group separators, and other formatting characters
            // Keep only digits, decimal separator, and negative sign
            text = new string(text.Where(c => char.IsDigit(c) || c == '.' || c == ',' || c == '-').ToArray());

            if (!double.TryParse(text, out var value)) return;

            // Handle percentage format
            if (format.Contains('P') || format.Contains('p') || format.Contains('%'))
            {
                // If the format is percentage but the value is already in decimal form (e.g., 0.25 for 25%)
                // we don't need to divide by 100 again
                if (value is > 0 and < 1 && !text.Contains('%'))
                {
                    // Value is already in decimal form, no need to divide
                }
                else if (!text.Contains('%'))
                {
                    // Convert from percentage to decimal (e.g., 25 to 0.25)
                    value /= 100;
                }
            }

            textBox.Text = value.ToString(format);
        };
    }

    /// <summary>
    ///     Gets or sets whether a TextBox should display a clear button to allow users to easily clear its content.
    /// </summary>
    /// <remarks>
    ///     This property enables a clear button functionality for TextBox controls. When set to true,
    ///     a clear button will be displayed that allows users to quickly clear the text content.
    ///     Example usage in XAML:
    ///     <code>
    /// &lt;TextBox Text="{Binding SearchText}" 
    ///          extensions:TextBoxAssist.Clearable="True" /&gt;
    /// </code>
    /// </remarks>
    public static readonly AttachedProperty<bool> ClearableProperty =
        AvaloniaProperty.RegisterAttached<TextBoxAssist, TextBox, bool>("Clearable");

    /// <summary>
    ///     Sets whether the specified TextBox should display a clear button.
    /// </summary>
    /// <param name="obj">The TextBox element to set the clearable property on.</param>
    /// <param name="value">True to enable the clear button; false to disable it.</param>
    public static void SetClearable(TextBox obj, bool value)
    {
        obj.SetValue(ClearableProperty, value);
    }

    /// <summary>
    ///     Gets whether the specified TextBox displays a clear button.
    /// </summary>
    /// <param name="obj">The TextBox element to get the clearable property from.</param>
    /// <returns>True if the clear button is enabled; false otherwise.</returns>
    public static bool GetClearable(TextBox obj)
    {
        return obj.GetValue(ClearableProperty);
    }
}