using Avalonia;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a card control with header, footer, and shadow properties.
/// </summary>
public class Card : ContentControl
{
    /// <summary>
    ///     Defines the <see cref="Header" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Header));

    /// <summary>
    ///     Gets or sets the header content of the card.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Footer" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Footer));

    /// <summary>
    ///     Gets or sets the footer content of the card.
    /// </summary>
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="HasShadow" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> HasShadowProperty =
        AvaloniaProperty.Register<Card, bool>(nameof(HasShadow));

    /// <summary>
    ///     Gets or sets a value indicating whether the card has a shadow.
    /// </summary>
    public bool HasShadow
    {
        get => GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }
}