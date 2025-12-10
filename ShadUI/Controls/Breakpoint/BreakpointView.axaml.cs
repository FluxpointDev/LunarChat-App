using Avalonia;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a view that can change its layout based on breakpoints.
/// </summary>
public class BreakpointView : Grid
{
    /// <summary>
    ///     Defines the <see cref="Breakpoint" /> property.
    /// </summary>
    public static readonly StyledProperty<Breakpoint> BreakpointProperty =
        AvaloniaProperty.Register<BreakpointViewPort, Breakpoint>(nameof(Breakpoint));

    /// <summary>
    ///     Gets or sets the breakpoint for the view.
    /// </summary>
    public Breakpoint Breakpoint
    {
        get => GetValue(BreakpointProperty);
        set => SetValue(BreakpointProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Invert" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> InvertProperty = AvaloniaProperty.Register<BreakpointViewPort, bool>(
        nameof(Invert));

    /// <summary>
    ///     Gets or sets a value indicating whether to invert the breakpoint logic.
    /// </summary>
    public bool Invert
    {
        get => GetValue(InvertProperty);
        set => SetValue(InvertProperty, value);
    }
}