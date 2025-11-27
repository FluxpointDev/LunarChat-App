using Avalonia.Controls;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// Works as <see cref="MarkdownTextBlock"/>
/// </summary>
public class InlineCollectionNode<TBlock> : BlockNode<TBlock> where TBlock : LeafBlock
{
    protected override MarkdownTextBlock TextBlock => textBlock;

    public override Control Control => textBlock;

    private readonly InlinesNode<ContainerInline> inlinesNode;
    private readonly MarkdownTextBlock textBlock;

    public InlineCollectionNode()
    {
        inlinesNode = new InlinesNode<ContainerInline>(new global::Avalonia.Controls.Documents.Span());
        textBlock = new MarkdownTextBlock
        {
            Inlines = inlinesNode.Inlines
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        TBlock block,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        return block.Inline is { } inline &&
            inlinesNode.Update(
                documentNode,
                inline,
                change,
                cancellationToken);
    }
}