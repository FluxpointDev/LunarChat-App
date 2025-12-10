// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Dialog options used to configure the dialog.
/// </summary>
public class DialogOptions
{
    /// <summary>
    ///     Determines whether the dialog can be dismissed other than clicking/tapping an action button.
    /// </summary>
    public bool Dismissible { get; set; }

    /// <summary>
    ///     Determines the maximum width of the dialog.
    /// </summary>
    public double MaxWidth { get; set; }

    /// <summary>
    ///     Determines the minimum width of the dialog.
    /// </summary>
    public double MinWidth { get; set; }
}