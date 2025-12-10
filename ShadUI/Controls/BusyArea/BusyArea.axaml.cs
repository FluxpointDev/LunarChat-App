using Avalonia;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a control that displays a busy indicator.
/// </summary>
public class BusyArea : ContentControl
{
    /// <summary>
    ///     Defines the <see cref="LoadingSize" /> property.
    /// </summary>
    public static readonly StyledProperty<double> LoadingSizeProperty =
        AvaloniaProperty.Register<BusyArea, double>(nameof(LoadingSize));

    /// <summary>
    ///     Gets or sets the size of the loading indicator.
    /// </summary>
    public double LoadingSize
    {
        get => GetValue(LoadingSizeProperty);
        set => SetValue(LoadingSizeProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="IsBusy" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsBusyProperty =
        AvaloniaProperty.Register<BusyArea, bool>(nameof(IsBusy));

    /// <summary>
    ///     Gets or sets a value indicating whether the control is busy.
    /// </summary>
    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }
}