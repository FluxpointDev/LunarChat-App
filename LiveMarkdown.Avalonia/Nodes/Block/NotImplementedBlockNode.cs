using Avalonia.Controls;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// A block node for Markdown blocks that are not yet implemented.
/// </summary>
public class NotImplementedBlockNode(Type markdownType) : BlockNode<Block>
{
    public override Control Control { get; } = new()
    {
        Classes = { "NotImplementedBlock" }
    };

    protected override bool UpdateCore(
        DocumentNode documentNode,
        Block block,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        return block.GetType() == markdownType;
    }
}