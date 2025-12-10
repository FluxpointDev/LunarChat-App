using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents an avatar control that displays an image.
/// </summary>
public class Avatar : TemplatedControl
{
    /// <summary>
    ///     Defines the <see cref="Source" /> property.
    /// </summary>
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<Avatar, IImage?>(nameof(Source));

    /// <summary>
    ///     Gets or sets the source image of the avatar.
    /// </summary>
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Fallback" /> property.
    /// </summary>
    public static readonly StyledProperty<string> FallbackProperty =
        AvaloniaProperty.Register<Avatar, string>(nameof(Fallback));

    /// <summary>
    ///     Gets or sets the fallback text of the avatar.
    /// </summary>
    public string Fallback
    {
        get => GetValue(FallbackProperty);
        set => SetValue(FallbackProperty, value);
    }
}