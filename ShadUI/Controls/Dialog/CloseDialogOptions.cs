// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Options for closing a dialog.
/// </summary>
public sealed class CloseDialogOptions
{
    /// <summary>
    ///     Indicates whether the dialog was closed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     Specifies whether all dialogs should be cleared when closing this dialog.
    /// </summary>
    public bool ClearAll { get; set; }
}