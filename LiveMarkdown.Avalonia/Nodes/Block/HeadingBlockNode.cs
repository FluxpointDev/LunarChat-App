using Avalonia.Controls;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public class HeadingBlockNode : BlockNode<HeadingBlock>
{
    public override Control Control { get; }

    private readonly InlineCollectionNode<HeadingBlock> headingInlines;

    public HeadingBlockNode()
    {
        headingInlines = new InlineCollectionNode<HeadingBlock>();
        Control = new Border
        {
            Classes = { "HeadingBlock" },
            Child = headingInlines.Control
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        HeadingBlock headingBlock,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        if (headingBlock.Inline is null) return false;

        if (!headingInlines.Update(documentNode, headingBlock, change, cancellationToken)) return false;

        cancellationToken.ThrowIfCancellationRequested();
        headingInlines.Control.Classes.EnsureClassName("Heading", headingBlock.Level);
        return true;
    }
}