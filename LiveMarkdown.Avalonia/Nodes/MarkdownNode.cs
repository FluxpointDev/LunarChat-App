using Markdig.Syntax;
using Markdig.Syntax.Inlines;

// ReSharper disable InconsistentNaming

namespace LiveMarkdown.Avalonia;

public abstract class MarkdownNode
{
    protected static IReadOnlyCollection<IMarkdownNodeFactory> NodeFactories => NodeFactoriesSet;

    private readonly static HashSet<IMarkdownNodeFactory> NodeFactoriesSet =
    [
        new MarkdownNodeFactory<AutolinkInlineNode>(),
        new MarkdownNodeFactory<CodeInlineNode>(),
        new MarkdownNodeFactory<ContainerInlineNode<ContainerInline>>(),
        new MarkdownNodeFactory<DelimiterInlineNode>(),
        new MarkdownNodeFactory<EmphasisInlineNode>(),
        new MarkdownNodeFactory<HtmlEntityInlineNode>(),
        new MarkdownNodeFactory<LineBreakInlineNode>(),
        new MarkdownNodeFactory<LinkInlineNode>(),
        new MarkdownNodeFactory<LiteralInlineNode>(),
        new MarkdownNodeFactory<TaskListNode>(),

        new MarkdownNodeFactory<AlertBlockNode>(),
        new MarkdownNodeFactory<CodeBlockNode>(),
        new MarkdownNodeFactory<HeadingBlockNode>(),
        new MarkdownNodeFactory<ListBlockNode>(),
        new MarkdownNodeFactory<ListItemBlockNode>(),
        new MarkdownNodeFactory<ParagraphBlockNode>(),
        new MarkdownNodeFactory<QuoteBlockNode>(),
        new MarkdownNodeFactory<TableCellNode>(),
        new MarkdownNodeFactory<TableNode>(),
        new MarkdownNodeFactory<ThematicBreakBlockNode>(),
    ];

    public static void Register<TNode>() where TNode : MarkdownNode, new()
    {
        NodeFactoriesSet.Add(new MarkdownNodeFactory<TNode>());
    }

    /// <summary>
    /// Gets the text block associated with this node, if any.
    /// </summary>
    protected virtual MarkdownTextBlock? TextBlock => null;

    /// <summary>
    /// records the source span of the block in the Markdown document.
    /// </summary>
    private SourceSpan span;

    private bool IsDirty(MarkdownObject markdownObject, in ObservableStringBuilderChangedEventArgs change)
    {
        return !span.Equals(markdownObject.Span) || span.End >= change.StartIndex && change.StartIndex + change.Length > span.Start;
    }

    public bool Update(
        DocumentNode documentNode,
        MarkdownObject markdownObject,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        if (!IsDirty(markdownObject, change))
        {
            // No need to update, the change does not affect this node
            return true;
        }

        var result = UpdateCore(documentNode, markdownObject, change, cancellationToken);
        span = markdownObject.Span;

        if (TextBlock is { } textBlock)
        {
            if (result)
            {
                textBlock.SourceSpan = span;
                documentNode.textBlocks.Add(textBlock);
            }
            else
            {
                documentNode.textBlocks.Remove(textBlock);
            }
        }

        MarkdownRenderer.VerboseLogger?.Log(
            this,
            "Updated {NodeName} {Result} {Span}",
            markdownObject.GetType().Name,
            result,
            markdownObject.Span);
        return result;
    }

    /// <summary>
    /// Updates the block with the given inlines and change information.
    /// </summary>
    /// <param name="documentNode"></param>
    /// <param name="markdownObject"></param>
    /// <param name="change"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if the block was updated successfully, false if it needs to be removed</returns>
    protected abstract bool UpdateCore(
        DocumentNode documentNode,
        MarkdownObject markdownObject,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken);
}
