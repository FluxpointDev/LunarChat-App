using Avalonia;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Useful extensions for the <see cref="Avalonia.Controls.ToggleSwitch" /> class.
/// </summary>
public static class ToggleSwitchAssist
{
    /// <summary>
    ///     Determines whether the toggle switch on/of content is right-aligned.
    /// </summary>
    public static readonly AttachedProperty<bool> RightAlignedContentProperty =
        AvaloniaProperty.RegisterAttached<ToggleSwitch, bool>("RightAlignedContent",
            typeof(ToggleSwitch));

    /// <summary>
    ///     Gets the value of the <see cref="RightAlignedContentProperty" />.
    /// </summary>
    /// <param name="toggleSwitch"></param>
    /// <returns></returns>
    public static bool GetRightAlignedContent(ToggleSwitch toggleSwitch) =>
        toggleSwitch.GetValue(RightAlignedContentProperty);

    /// <summary>
    ///     Sets the value of the <see cref="RightAlignedContentProperty" />.
    /// </summary>
    /// <param name="toggleSwitch"></param>
    /// <param name="value"></param>
    public static void SetRightAlignedContent(ToggleSwitch toggleSwitch, bool value)
    {
        toggleSwitch.SetValue(RightAlignedContentProperty, value);
    }
}