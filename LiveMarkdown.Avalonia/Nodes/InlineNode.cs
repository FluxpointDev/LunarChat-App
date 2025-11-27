using System.Runtime.CompilerServices;
using Avalonia.Controls.Documents;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public abstract class InlineNode : MarkdownNode
{
    public abstract Inline Inline { get; }

    protected static InlineNode CreateInlineNode(
        DocumentNode documentNode,
        Markdig.Syntax.Inlines.Inline inline,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        var type = inline.GetType();

        // First try to find an exact match, then try to find a compatible type
        var node = NodeFactories
                .OfType<IMarkdownNodeFactory<InlineNode>>()
                .Where(f => f.MarkdownType.IsAssignableFrom(type))
                .OrderBy(f => f)
                .Select(f => f.CreateNode())
                .FirstOrDefault()
            ?? new NotImplementedInlineNode(inline.GetType());

        node.Update(documentNode, inline, change, cancellationToken);
        return node;
    }
}

public abstract class InlineNode<TInline> : InlineNode where TInline : Markdig.Syntax.Inlines.Inline
{
    protected sealed override bool UpdateCore(
        DocumentNode documentNode,
        MarkdownObject markdownObject,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        return markdownObject.GetType() == typeof(TInline) &&
            UpdateCore(documentNode, Unsafe.As<TInline>(markdownObject), change, cancellationToken);
    }

    protected abstract bool UpdateCore(
        DocumentNode documentNode,
        TInline inline,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken);
}