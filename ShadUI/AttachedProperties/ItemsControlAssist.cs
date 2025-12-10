using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Useful extension methods for <see cref="Avalonia.Controls.ItemsControl" />
/// </summary>
public static class ItemsControlAssist
{
    /// <summary>
    ///     Gets or sets if scroll is animated.
    /// </summary>
    public static readonly AttachedProperty<bool> AnimatedScrollProperty =
        AvaloniaProperty.RegisterAttached<ItemsControl, bool>("AnimatedScroll",
            typeof(ItemsControl));

    static ItemsControlAssist()
    {
        AnimatedScrollProperty.Changed.AddClassHandler<ItemsControl>(HandleAnimatedScrollChanged);
    }

    private static void HandleAnimatedScrollChanged(ItemsControl interactElem,
        AvaloniaPropertyChangedEventArgs args)
    {
        if (GetAnimatedScroll(interactElem))
        {
            interactElem.AttachedToVisualTree += (_, _) =>
                ElementComposition.GetElementVisual(interactElem).MakeScrollable();
        }
    }

    /// <summary>
    ///     Gets the value of <see cref="AnimatedScrollProperty" />
    /// </summary>
    /// <param name="wrap"></param>
    /// <returns></returns>
    public static bool GetAnimatedScroll(ItemsControl wrap) => wrap.GetValue(AnimatedScrollProperty);

    /// <summary>
    ///     Sets the value of <see cref="AnimatedScrollProperty" />
    /// </summary>
    /// <param name="wrap"></param>
    /// <param name="value"></param>
    public static void SetAnimatedScroll(ItemsControl wrap, bool value)
    {
        wrap.SetValue(AnimatedScrollProperty, value);
    }
}