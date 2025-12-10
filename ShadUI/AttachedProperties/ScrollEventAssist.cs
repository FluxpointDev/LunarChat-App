using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides attached properties for handling scroll events in popups and overlays.
///     <para>
///         This class is particularly useful for preventing scroll events from propagating
///         to parent ScrollViewers when using popup controls like AutoCompleteBox, ComboBox,
///         and SimpleDropdown.
///     </para>
///     <para>
///         Usage: Set <see cref="PreventScrollPropagationProperty" /> to <c>true</c> on any
///         control to stop scroll events from bubbling up to parent controls.
///     </para>
/// </summary>
public static class ScrollEventAssist
{
    /// <summary>
    ///     Gets or sets whether scroll events should be prevented from propagating to parent controls.
    /// </summary>
    public static readonly AttachedProperty<bool> PreventScrollPropagationProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("PreventScrollPropagation",
            typeof(ScrollEventAssist));

    static ScrollEventAssist()
    {
        PreventScrollPropagationProperty.Changed.AddClassHandler<Control>(HandlePreventScrollPropagationChanged);
    }

    /// <summary>
    ///     Gets the value of <see cref="PreventScrollPropagationProperty" />
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>The prevent scroll propagation value.</returns>
    public static bool GetPreventScrollPropagation(Control control)
    {
        return control.GetValue(PreventScrollPropagationProperty);
    }

    /// <summary>
    ///     Sets the value of <see cref="PreventScrollPropagationProperty" />
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="value">The prevent scroll propagation value to set.</param>
    public static void SetPreventScrollPropagation(Control control, bool value)
    {
        control.SetValue(PreventScrollPropagationProperty, value);
    }

    private static void HandlePreventScrollPropagationChanged(Control control, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is not bool preventPropagation) return;

        if (preventPropagation)
        {
            control.PointerWheelChanged += OnPointerWheelChanged;
        }
        else
        {
            control.PointerWheelChanged -= OnPointerWheelChanged;
        }
    }

    private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        e.Handled = true;
    }
}