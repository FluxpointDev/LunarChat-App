using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     A smart key binding that provides special handling for TextBox controls while maintaining regular key binding
///     functionality.
///     When a TextBox is focused, the key event is first passed to the TextBox before executing the command.
/// </summary>
public class SmartKeyBinding : KeyBinding, ICommand
{
    /// <summary>
    ///     The styled property for the smart command that will be executed.
    /// </summary>
    public static readonly StyledProperty<ICommand> SmartCommandProperty =
        AvaloniaProperty.Register<SmartKeyBinding, ICommand>(nameof(SmartCommand));

    /// <summary>
    ///     Gets or sets the command that will be executed when the key binding is triggered.
    ///     This command receives special handling for TextBox controls.
    /// </summary>
    /// <value>
    ///     The ICommand to execute when the key binding is triggered.
    /// </value>
    public ICommand SmartCommand
    {
        get => GetValue(SmartCommandProperty);
        set => SetValue(SmartCommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the command for the key binding.
    ///     It is recommended to use <see cref="SmartCommand" /> instead for proper TextBox handling.
    /// </summary>
    [Obsolete(
        "The Command property is not supported in SmartKeyBinding. Use SmartCommand property instead for proper TextBox handling and focus-aware key binding behavior.",
        true)]
    public new ICommand Command
    {
        get => base.Command;
        set => base.Command = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SmartKeyBinding" /> class.
    ///     Sets up the internal command handling.
    /// </summary>
    public SmartKeyBinding()
    {
        base.Command = this;
    }

    /// <summary>
    ///     Determines whether the command can be executed in its current state.
    ///     Returns true if a TextBox is focused or if the underlying command can execute.
    /// </summary>
    /// <param name="parameter">
    ///     Data used by the command. If the command does not require data, this parameter can be set to
    ///     null.
    /// </param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object? parameter)
    {
        var focusManager = Application.Current.GetTopLevel()?.FocusManager;
        return focusManager?.GetFocusedElement() is TextBox || SmartCommand.CanExecute(parameter);
    }

    /// <summary>
    ///     Executes the command with the specified parameter.
    ///     If a TextBox is focused, the key event is first passed to the TextBox.
    ///     If the TextBox doesn't handle the event, the command is then executed.
    /// </summary>
    /// <param name="parameter">
    ///     Data used by the command. If the command does not require data, this parameter can be set to
    ///     null.
    /// </param>
    public void Execute(object? parameter)
    {
        var focusManager = Application.Current.GetTopLevel()?.FocusManager;
        if (focusManager?.GetFocusedElement() is TextBox textBox)
        {
            var ev = new KeyEventArgs
            {
                Key = Gesture.Key,
                KeyModifiers = Gesture.KeyModifiers,
                RoutedEvent = InputElement.KeyDownEvent
            };
            textBox.RaiseEvent(ev);
            if (!ev.Handled && CanExecute(parameter)) SmartCommand.Execute(parameter);
        }
        else
        {
            SmartCommand.Execute(parameter);
        }
    }

    /// <summary>
    ///     Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => SmartCommand.CanExecuteChanged += value;
        remove => SmartCommand.CanExecuteChanged -= value;
    }
}