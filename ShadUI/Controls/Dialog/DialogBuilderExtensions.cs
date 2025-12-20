// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Fluent API for building dialogs.
/// </summary>
public static class DialogBuilderExtensions
{
    /// <summary>
    ///     Creates a simple dialog.
    /// </summary>
    /// <param name="manager">The <see cref="DialogManager" /></param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The dialog message</param>
    /// <returns>A new instance of <see cref="SimpleDialogBuilder" /></returns>
    public static SimpleDialogBuilder CreateDialog(this DialogManager manager, string title, string message)
        => new SimpleDialogBuilder(manager).CreateDialog(title, message);

    /// <summary>
    ///     Sets the primary button of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="callback">The method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Primary" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithPrimaryButton(this SimpleDialogBuilder builder, string text,
        Action? callback = null,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Primary)
    {
        builder.PrimaryButtonText = text;
        builder.PrimaryCallback = callback;
        builder.PrimaryButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the primary button of the dialog with an asynchronous callback.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="asyncCallback">The asynchronous method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Primary" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithPrimaryButton(this SimpleDialogBuilder builder, string text,
        Func<Task>? asyncCallback = null,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Primary)
    {
        builder.PrimaryButtonText = text;
        builder.PrimaryCallbackAsync = asyncCallback;
        builder.PrimaryButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the secondary button of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="callback">The method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Secondary" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithSecondaryButton(this SimpleDialogBuilder builder, string text,
        Action? callback = null,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Secondary)
    {
        builder.SecondaryButtonText = text;
        builder.SecondaryCallback = callback;
        builder.SecondaryButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the secondary button of the dialog with an asynchronous callback.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="asyncCallback">The asynchronous method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Secondary" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithSecondaryButton(this SimpleDialogBuilder builder, string text,
        Func<Task>? asyncCallback = null,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Secondary)
    {
        builder.SecondaryButtonText = text;
        builder.SecondaryCallbackAsync = asyncCallback;
        builder.SecondaryButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the tertiary button of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="callback">The method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Outline" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithTertiaryButton(this SimpleDialogBuilder builder, string text,
        Action? callback = null,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Outline)
    {
        builder.TertiaryButtonText = text;
        builder.TertiaryCallback = callback;
        builder.TertiaryButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the tertiary button of the dialog with an asynchronous callback.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="asyncCallback">The asynchronous method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Outline" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithTertiaryButton(this SimpleDialogBuilder builder, string text,
        Func<Task>? asyncCallback = null,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Outline)
    {
        builder.TertiaryButtonText = text;
        builder.TertiaryCallbackAsync = asyncCallback;
        builder.TertiaryButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the cancel button of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Outline" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithCancelButton(this SimpleDialogBuilder builder, string text,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Outline)
    {
        builder.CancelButtonText = text;
        builder.CancelButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the cancel button of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="callback">The method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Outline" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithCancelButton(this SimpleDialogBuilder builder, string text, Action callback,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Outline)
    {
        builder.CancelButtonText = text;
        builder.CancelCallback = callback;
        builder.CancelButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Sets the cancel button of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="text">The button text</param>
    /// <param name="asyncCallback">The asynchronous method that is called once the button is clicked</param>
    /// <param name="buttonStyle">The style of the button. The default is <see cref="DialogButtonStyle.Outline" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithCancelButton(this SimpleDialogBuilder builder, string text,
        Func<Task>? asyncCallback,
        DialogButtonStyle buttonStyle = DialogButtonStyle.Outline)
    {
        builder.CancelButtonText = text;
        builder.CancelCallbackAsync = asyncCallback;
        builder.CancelButtonStyle = buttonStyle;
        return builder;
    }

    /// <summary>
    ///     Makes the dialog dismissible by clicking outside or pressing escape.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder Dismissible(this SimpleDialogBuilder builder)
    {
        builder.Options.Dismissible = true;
        return builder;
    }

    /// <summary>
    ///     Sets the maximum width of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="maxWidth">The maximum width in pixels</param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithMaxWidth(this SimpleDialogBuilder builder, double maxWidth)
    {
        builder.Options.MaxWidth = maxWidth;
        return builder;
    }

    /// <summary>
    ///     Sets the minimum width of the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    /// <param name="minWidth">The minimum width in pixels</param>
    /// <returns>The modified <see cref="SimpleDialogBuilder" /> instance</returns>
    public static SimpleDialogBuilder WithMinWidth(this SimpleDialogBuilder builder, double minWidth)
    {
        builder.Options.MinWidth = minWidth;
        return builder;
    }

    /// <summary>
    ///     Shows the dialog.
    /// </summary>
    /// <param name="builder">The <see cref="SimpleDialogBuilder" /></param>
    public static void Show(this SimpleDialogBuilder builder)
    {
        builder.Show();
    }

    /// <summary>
    ///     Creates a dialog with a custom context.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="manager">The <see cref="DialogManager" /></param>
    /// <param name="context">The dialog context</param>
    /// <returns>A new instance of <see cref="DialogBuilder{TContext}" /></returns>
    public static DialogBuilder<TContext> CreateDialog<TContext>(this DialogManager manager, TContext context)
    {
        return new DialogBuilder<TContext>(manager).CreateDialog(context);
    }

    /// <summary>
    ///     Sets the success callback for the dialog.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="callback">The method that is called on successful completion</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithSuccessCallback<TContext>(this DialogBuilder<TContext> builder,
        Action callback)
    {
        builder.OnSuccessCallback = callback;
        return builder;
    }

    /// <summary>
    ///     Sets the success callback for the dialog with context parameter.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="callback">The method that is called on successful completion, receiving the dialog context as a parameter</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithSuccessCallback<TContext>(this DialogBuilder<TContext> builder,
        Action<TContext> callback)
    {
        builder.OnSuccessWithContextCallback = ctx => callback((TContext)ctx);
        return builder;
    }

    /// <summary>
    ///     Sets the success callback for the dialog with an asynchronous callback.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="callback">The asynchronous method that is called on successful completion</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithSuccessCallback<TContext>(this DialogBuilder<TContext> builder,
        Func<Task> callback)
    {
        builder.OnSuccessAsyncCallback = callback;
        return builder;
    }

    /// <summary>
    ///     Sets the success callback for the dialog with an asynchronous callback that receives the dialog context.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="callback">
    ///     The asynchronous method that is called on successful completion, receiving the dialog context as
    ///     a parameter
    /// </param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithSuccessCallback<TContext>(this DialogBuilder<TContext> builder,
        Func<TContext, Task> callback)
    {
        builder.OnSuccessWithContextAsyncCallback = ctx => callback((TContext)ctx);
        return builder;
    }

    /// <summary>
    ///     Sets the cancel callback for the dialog.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="callback">The method that is called when the dialog is cancelled</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithCancelCallback<TContext>(this DialogBuilder<TContext> builder,
        Action callback)
    {
        builder.OnCancelCallback = callback;
        return builder;
    }

    /// <summary>
    ///     Sets the cancel callback for the dialog with an asynchronous callback.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="callback">The asynchronous method that is called when the dialog is cancelled</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithCancelCallback<TContext>(this DialogBuilder<TContext> builder,
        Func<Task> callback)
    {
        builder.OnCancelAsyncCallback = callback;
        return builder;
    }

    /// <summary>
    ///     Makes the dialog dismissible by clicking outside or pressing escape. If set to true, this will take precedence over
    ///     toggling <see cref="DialogManager.PreventDismissal()" />
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> Dismissible<TContext>(this DialogBuilder<TContext> builder)
    {
        builder.Options.Dismissible = true;
        return builder;
    }

    /// <summary>
    ///     Sets the maximum width of the dialog.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="maxWidth">The maximum width in pixels</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithMaxWidth<TContext>(this DialogBuilder<TContext> builder, double maxWidth)
    {
        builder.Options.MaxWidth = maxWidth;
        return builder;
    }

    /// <summary>
    ///     Sets the minimum width of the dialog.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    /// <param name="minWidth">The minimum width in pixels</param>
    /// <returns>The modified <see cref="DialogBuilder{TContext}" /> instance</returns>
    public static DialogBuilder<TContext> WithMinWidth<TContext>(this DialogBuilder<TContext> builder, double minWidth)
    {
        builder.Options.MinWidth = minWidth;
        return builder;
    }

    /// <summary>
    ///     Shows the dialog.
    /// </summary>
    /// <typeparam name="TContext">The type of the dialog context</typeparam>
    /// <param name="builder">The <see cref="DialogBuilder{TContext}" /></param>
    public static void Show<TContext>(this DialogBuilder<TContext> builder)
    {
        builder.Show();
    }
}