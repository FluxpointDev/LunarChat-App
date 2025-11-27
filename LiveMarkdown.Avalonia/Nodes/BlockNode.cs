using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public abstract class BlockNode : MarkdownNode
{
    public abstract Control Control { get; }

    protected static BlockNode CreateBlockNode(
        DocumentNode documentNode,
        Block block,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        var type = block.GetType();

        // First try to find an exact match, then try to find a compatible type
        var node = NodeFactories
                .OfType<IMarkdownNodeFactory<BlockNode>>()
                .Where(f => f.MarkdownType.IsAssignableFrom(type))
                .OrderBy(f => f)
                .Select(f => f.CreateNode())
                .FirstOrDefault()
            ?? new NotImplementedBlockNode(block.GetType());

        node.Update(documentNode, block, change, cancellationToken);
        return node;
    }
}

public abstract class BlockNode<TBlock> : BlockNode where TBlock : Block
{
    /// <summary>
    /// Determines whether the given block matches the type TBlock.
    /// Default implementation checks for exact type match.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    protected virtual bool MatchesBlock(TBlock block) => block.GetType() == typeof(TBlock);

    protected sealed override bool UpdateCore(
        DocumentNode documentNode,
        MarkdownObject markdownObject,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        return markdownObject is TBlock block &&
            MatchesBlock(block) &&
            UpdateCore(documentNode, Unsafe.As<TBlock>(markdownObject), change, cancellationToken);
    }

    protected abstract bool UpdateCore(
        DocumentNode documentNode,
        TBlock block,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken);
}