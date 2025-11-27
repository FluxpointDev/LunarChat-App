using Avalonia.Controls;
using Avalonia.Layout;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public abstract class ContainerBlockNode<TContainerBlock> : BlockNode<TContainerBlock> where TContainerBlock : ContainerBlock
{
    public override Control Control => container;

    /// <summary>
    /// The container that holds the child block nodes.
    /// </summary>
    protected readonly StackPanel container;

    /// <summary>
    /// The proxy that manages the child block nodes.
    /// </summary>
    protected readonly MarkdownRenderer.BlocksProxy proxy;

    protected ContainerBlockNode()
    {
        container = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        proxy = new MarkdownRenderer.BlocksProxy(container.Children);
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        TContainerBlock containerBlock,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        if (containerBlock.Count == 0) return false;

        var i = 0;
        for (; i < containerBlock.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var block = containerBlock[i];

            if (i < proxy.Count)
            {
                var oldNode = proxy[i];
                if (oldNode.Update(documentNode, block, change, cancellationToken)) continue;

                // if Update returned false, it means the block needs to be removed
                var newNode = CreateBlockNode(documentNode, block, change, cancellationToken);
                proxy[i] = newNode;
            }
            else
            {
                var newNode = CreateBlockNode(documentNode, block, change, cancellationToken);
                proxy.Add(newNode);
            }
        }

        for (var j = proxy.Count - 1; j >= i; j--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            proxy.RemoveAt(j);
        }

        return true;
    }
}