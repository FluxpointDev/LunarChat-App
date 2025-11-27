using Avalonia.Controls.Documents;
using Inline = Markdig.Syntax.Inlines.Inline;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// A node that contains multiple inline nodes (Like a Span or InlineHyperlink).
/// </summary>
public class InlinesNode<TInline> : InlineNode<TInline> where TInline : Inline
{
    protected override MarkdownTextBlock? TextBlock { get; }

    public override global::Avalonia.Controls.Documents.Inline Inline { get; }

    public InlineCollection Inlines { get; }

    private readonly MarkdownRenderer.InlinesProxy proxy;

    public InlinesNode(Span span) : this(span, span.Inlines) { }

    protected InlinesNode(InlineHyperlink inlineHyperlink) : this(inlineHyperlink, inlineHyperlink.Inlines)
    {
        TextBlock = inlineHyperlink.TextBlock;
    }

    private InlinesNode(global::Avalonia.Controls.Documents.Inline inline, InlineCollection inlines)
    {
        Inline = inline;
        Inlines = inlines;
        proxy = new MarkdownRenderer.InlinesProxy(inlines);
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        TInline inlines,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        var i = -1;
        foreach (var inline in (IEnumerable<Inline>)inlines)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Add new inline
            if (proxy.Count > ++i)
            {
                // Update existing inline
                var oldInlineNode = proxy[i];

                // if Update returned true, it means the block was updated successfully
                if (oldInlineNode.Update(documentNode, inline, change, cancellationToken)) continue;

                // else, remove the old node and create a new one
                var newInlineNode = CreateInlineNode(documentNode, inline, change, cancellationToken);
                proxy[i] = newInlineNode;
            }
            else
            {
                var newInlineNode = CreateInlineNode(documentNode, inline, change, cancellationToken);
                proxy.Add(newInlineNode);
            }
        }

        while (proxy.Count > i + 1)
        {
            cancellationToken.ThrowIfCancellationRequested();
            proxy.RemoveAt(proxy.Count - 1);
        }

        return i >= 0; // Return true if at least one inline was processed
    }
}