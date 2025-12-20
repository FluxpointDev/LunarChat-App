// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Builds a simple dialog.
/// </summary>
public sealed class SimpleDialogBuilder
{
    private readonly DialogManager _manager;
    private SimpleDialog? _dialog;

    internal SimpleDialogBuilder(DialogManager manager)
    {
        _manager = manager;
    }

    internal string PrimaryButtonText { get; set; } = string.Empty;
    internal Action? PrimaryCallback { get; set; }
    internal Func<Task>? PrimaryCallbackAsync { get; set; }
    internal DialogButtonStyle PrimaryButtonStyle { get; set; } = DialogButtonStyle.Primary;
    internal string SecondaryButtonText { get; set; } = string.Empty;
    internal Action? SecondaryCallback { get; set; }
    internal Func<Task>? SecondaryCallbackAsync { get; set; }
    internal DialogButtonStyle SecondaryButtonStyle { get; set; } = DialogButtonStyle.Secondary;
    internal string TertiaryButtonText { get; set; } = string.Empty;
    internal Action? TertiaryCallback { get; set; }
    internal Func<Task>? TertiaryCallbackAsync { get; set; }
    internal DialogButtonStyle TertiaryButtonStyle { get; set; } = DialogButtonStyle.Outline;
    internal string CancelButtonText { get; set; } = "Cancel";
    internal Action? CancelCallback { get; set; }
    internal Func<Task>? CancelCallbackAsync { get; set; }
    internal DialogButtonStyle CancelButtonStyle { get; set; } = DialogButtonStyle.Outline;
    internal DialogOptions Options { get; } = new();

    internal SimpleDialogBuilder CreateDialog(string title, string message)
    {
        _dialog = new SimpleDialog(_manager)
        {
            Title = title,
            Message = message
        };
        return this;
    }

    internal void Show()
    {
        _dialog ??= new SimpleDialog(_manager);

        _dialog.PrimaryButtonText = PrimaryButtonText;
        _dialog.PrimaryCallback = PrimaryCallback;
        _dialog.PrimaryCallbackAsync = PrimaryCallbackAsync;
        _dialog.PrimaryButtonStyle = PrimaryButtonStyle;
        _dialog.SecondaryButtonText = SecondaryButtonText;
        _dialog.SecondaryCallback = SecondaryCallback;
        _dialog.SecondaryCallbackAsync = SecondaryCallbackAsync;
        _dialog.SecondaryButtonStyle = SecondaryButtonStyle;
        _dialog.TertiaryButtonText = TertiaryButtonText;
        _dialog.TertiaryCallback = TertiaryCallback;
        _dialog.TertiaryCallbackAsync = TertiaryCallbackAsync;
        _dialog.TertiaryButtonStyle = TertiaryButtonStyle;
        _dialog.CancelButtonText = CancelButtonText;
        _dialog.CancelCallback = CancelCallback;
        _dialog.CancelCallbackAsync = CancelCallbackAsync;
        _dialog.CancelButtonStyle = CancelButtonStyle;

        _dialog.SetId(Options);
        _manager.Show(_dialog, Options);
    }
}