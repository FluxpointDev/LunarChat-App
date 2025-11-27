// @author https://github.com/DearVa
// @author https://github.com/AuroraZiling
// @author https://github.com/SlimeNull

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Threading;
using Markdig;

namespace LiveMarkdown.Avalonia;

public partial class MarkdownRenderer : Control
{
    /// <summary>
    /// Defines the <see cref="MarkdownBuilder"/> property.
    /// </summary>
    public static readonly DirectProperty<MarkdownRenderer, ObservableStringBuilder?> MarkdownBuilderProperty =
        AvaloniaProperty.RegisterDirect<MarkdownRenderer, ObservableStringBuilder?>(
            nameof(MarkdownBuilder),
            o => o.MarkdownBuilder,
            (o, v) => o.MarkdownBuilder = v);

    /// <summary>
    /// An <see cref="ObservableStringBuilder"/> containing the Markdown text to render.
    /// If set, the control will listen to changes in the builder and update the rendering accordingly.
    /// </summary>
    public ObservableStringBuilder? MarkdownBuilder
    {
        get;
        set
        {
            var oldValue = field;
            if (!SetAndRaise(MarkdownBuilderProperty, ref field, value)) return;

            if (oldValue is not null) oldValue.Changed -= CommitChange;
            if (value is not null)
            {
                value.Changed += CommitChange;
                CommitChange(new ObservableStringBuilderChangedEventArgs(value.ToString(), 0, value.Length));
            }
        }
    }

    /// <summary>
    /// Defines the <see cref="ImageBasePath"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> ImageBasePathProperty =
        AvaloniaProperty.Register<MarkdownRenderer, string?>(nameof(ImageBasePath));

    /// <summary>
    /// Base path for resolving relative image URLs.
    /// If not set, relative image URLs will not be resolved.
    /// Changing this property will not affect already rendered images.
    /// </summary>
    public string? ImageBasePath
    {
        get => GetValue(ImageBasePathProperty);
        set => SetValue(ImageBasePathProperty, value);
    }

    /// <summary>
    /// Raised when an inline hyperlink is clicked.
    /// </summary>
    public event EventHandler<InlineHyperlinkClickedEventArgs>? InlineHyperlinkClick
    {
        add => AddHandler(InlineHyperlink.ClickEvent, value);
        remove => RemoveHandler(InlineHyperlink.ClickEvent, value);
    }

    public static readonly StyledProperty<ICommand?> InlineHyperlinkCommandProperty = AvaloniaProperty.Register<MarkdownRenderer, ICommand?>(
        nameof(InlineHyperlinkCommand));

    /// <summary>
    /// Command that is executed when an inline hyperlink is clicked. Command parameter is <see cref="InlineHyperlinkClickedEventArgs"/>.
    /// </summary>
    public ICommand? InlineHyperlinkCommand
    {
        get => GetValue(InlineHyperlinkCommandProperty);
        set => SetValue(InlineHyperlinkCommandProperty, value);
    }

    private ObservableStringBuilderChangedEventArgs? pendingChange;

    private readonly DocumentNode documentNode;
    private readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseCodeBlockSpanFixer()
        .Build();

    internal static readonly ParametrizedLogger? VerboseLogger;

    static MarkdownRenderer()
    {
        VerboseLogger = Logger.TryGet(LogEventLevel.Verbose, nameof(MarkdownRenderer));

        InlineHyperlink.ClickEvent.AddClassHandler<MarkdownRenderer>(HandleInlineHyperlinkClick);
        RequestBringIntoViewEvent.AddClassHandler<MarkdownRenderer>(BringIntoViewRequested);
    }

    private static void HandleInlineHyperlinkClick(MarkdownRenderer sender, InlineHyperlinkClickedEventArgs args)
    {
        if (args.Handled ||
            sender.InlineHyperlinkCommand is not { } inlineHyperlinkCommand ||
            !inlineHyperlinkCommand.CanExecute(args)) return;

        inlineHyperlinkCommand.Execute(args);
    }

    private static void BringIntoViewRequested(MarkdownRenderer sender, RequestBringIntoViewEventArgs args)
    {
        // ignore requests from children
        args.Handled = true;
    }

    public MarkdownRenderer()
    {
        documentNode = new DocumentNode(this);
        LogicalChildren.Add(documentNode.Control);
        VisualChildren.Add(documentNode.Control);

        AddHandler(PointerPressedEvent, HandlePointerPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, HandlePointerMoved, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, HandlePointerReleased, RoutingStrategies.Tunnel);
        AddHandler(KeyDownEvent, HandleKeyDown, RoutingStrategies.Tunnel);
    }

    protected override async void ArrangeCore(Rect finalRect)
    {
        if (pendingChange is { } e)
        {
            pendingChange = null;

            try
            {
                var markdown = e.NewString;
                var time = DateTimeOffset.UtcNow;
                var document = await Task.Run(() => Markdown.Parse(markdown, pipeline));
                VerboseLogger?.Log(this, "Parse markdown in {TotalMicroseconds} ms.", (DateTimeOffset.UtcNow - time).TotalMilliseconds);

                time = DateTimeOffset.UtcNow;
                documentNode.Update(documentNode, document, e, CancellationToken.None);
                VerboseLogger?.Log(this, "Render markdown in {TotalMicroseconds} ms.", (DateTimeOffset.UtcNow - time).TotalMilliseconds);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                await Console.Error.WriteAsync($"Error while rendering markdown: {ex.Message}");
            }
        }

        base.ArrangeCore(finalRect);
    }

    private void CommitChange(in ObservableStringBuilderChangedEventArgs e)
    {
        Dispatcher.UIThread.VerifyAccess();

        if (pendingChange is null) pendingChange = e;
        else
        {
            pendingChange = new ObservableStringBuilderChangedEventArgs(
                e.NewString,
                Math.Min(pendingChange.Value.StartIndex, e.StartIndex),
                Math.Max(pendingChange.Value.Length, e.Length));
        }

        InvalidateArrange();
    }
}


/// <summary>
/// Event arguments for the InlineHyperlinkClicked event.
/// </summary>
/// <param name="routedEvent"></param>
/// <param name="source">Must be <see cref="InlineHyperlink"/></param>
/// <param name="href"></param>
public class InlineHyperlinkClickedEventArgs(RoutedEvent routedEvent, object source, Uri? href) : RoutedEventArgs(routedEvent, source)
{
    public Uri? HRef => href;
}