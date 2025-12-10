using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Useful extensions for the <see cref="Avalonia.Controls.Button" /> class.
/// </summary>
public static class ButtonAssist
{
    /// <summary>
    ///     Show or hide the progress indicator.
    /// </summary>
    public static readonly AttachedProperty<bool> ShowProgressProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("ShowProgress", typeof(Button));

    /// <summary>
    ///     Get the value of the <see cref="ShowProgressProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <returns></returns>
    public static bool GetShowProgress(Button textBox)
    {
        return textBox.GetValue(ShowProgressProperty);
    }

    /// <summary>
    ///     Set the value of the <see cref="ShowProgressProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <param name="value"></param>
    public static void SetShowProgress(Button textBox, bool value)
    {
        textBox.SetValue(ShowProgressProperty, value);
    }

    /// <summary>
    ///     Add button icon.
    /// </summary>
    public static readonly AttachedProperty<object> IconProperty =
        AvaloniaProperty.RegisterAttached<Button, object>("Icon", typeof(Button));

    /// <summary>
    ///     Get the value of the <see cref="IconProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <returns></returns>
    public static object GetIcon(Button textBox)
    {
        return textBox.GetValue(IconProperty);
    }

    /// <summary>
    ///     Set the value of the <see cref="IconProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <param name="value"></param>
    public static void SetIcon(Button textBox, object value)
    {
        textBox.SetValue(IconProperty, value);
    }

    /// <summary>
    ///     Attached property for setting the hover background brush of a <see cref="Button" />.
    /// </summary>
    public static readonly AttachedProperty<IBrush> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush>("HoverBackground", typeof(Button));

    /// <summary>
    ///     Sets the hover background brush for the specified <see cref="Button" />.
    /// </summary>
    /// <param name="btn">The button to set the hover background for.</param>
    /// <param name="value">The brush to use as the hover background.</param>
    public static void SetHoverBackground(Button btn, IBrush value)
    {
        btn.SetValue(HoverBackgroundProperty, value);
    }

    /// <summary>
    ///     Gets the hover background brush for the specified <see cref="Button" />.
    /// </summary>
    /// <param name="btn">The button to get the hover background from.</param>
    /// <returns>The brush used as the hover background.</returns>
    public static IBrush GetHoverBackground(Button btn)
    {
        return btn.GetValue(HoverBackgroundProperty);
    }
}