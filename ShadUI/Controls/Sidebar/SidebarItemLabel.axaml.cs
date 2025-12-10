using Avalonia;
using Avalonia.Controls.Primitives;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a label item within a sidebar control.
/// </summary>
public class SidebarItemLabel : TemplatedControl
{
    /// <summary>
    ///     Defines the <see cref="Text" /> property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<SidebarItemLabel, string?>(
        nameof(Text));

    /// <summary>
    ///     Gets or sets the text content of the sidebar item label.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Expanded" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ExpandedProperty = AvaloniaProperty.Register<SidebarItemLabel, bool>(
        nameof(Expanded));

    /// <summary>
    ///     Gets or sets a value indicating whether the sidebar is expanded.
    /// </summary>
    public bool Expanded
    {
        get => GetValue(ExpandedProperty);
        set => SetValue(ExpandedProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="AsSeparator" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> AsSeparatorProperty = AvaloniaProperty.Register<SidebarItemLabel, bool>(
        nameof(AsSeparator));

    /// <summary>
    ///     Gets or sets a value indicating whether the label should be displayed as a separator.
    /// </summary>
    public bool AsSeparator
    {
        get => GetValue(AsSeparatorProperty);
        set => SetValue(AsSeparatorProperty, value);
    }
}