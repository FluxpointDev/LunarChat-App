using Avalonia.Controls;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public class ListBlockNode : BlockNode<ListBlock>
{
    public override Control Control => grid;

    private readonly Grid grid;
    private readonly MarkdownRenderer.BlocksProxy proxy;

    public ListBlockNode()
    {
        grid = new Grid
        {
            Classes = { "ListBlock" },
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition()
            }
        };
        proxy = new MarkdownRenderer.BlocksProxy(grid.Children);
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        ListBlock listBlock,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        if (listBlock.Count == 0) return false;

        var number = 1;
        for (var i = 0; i < listBlock.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (grid.RowDefinitions.Count <= i)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            var itemBlock = listBlock[i];
            var numberIndex = i * 2;
            
            TextBlock itemControl;
            if (proxy.Count > numberIndex && proxy[numberIndex].Control is TextBlock existingItemControl)
            {
                // existing item block node, update it
                itemControl = existingItemControl;
            }
            else
            {
                // create a new item block node
                itemControl = new TextBlock
                {
                    Classes = { listBlock.IsOrdered ? "ListBlockNumber" : "ListBlockBullet" },
                };

                if (proxy.Count > numberIndex)
                {
                    // replace the existing item block node
                    proxy.SetControlAt(numberIndex, itemControl);
                }
                else
                {
                    // add a new item block node
                    proxy.Add(itemControl);
                }
            }

            Grid.SetRow(itemControl, i);
            Grid.SetColumn(itemControl, 0);
            if (listBlock.IsOrdered)
            {
                itemControl.Text = $"{number++}.";
            }
            else
            {
                itemControl.Classes.EnsureClassName("Level", (listBlock.Column / 2) % 4);
            }

            // item part
            var itemIndex = i * 2 + 1;
            if (proxy.Count > itemIndex)
            {
                // existing item block node, update it
                var oldItemBlockNode = proxy[itemIndex];

                // if Update returned true, it means the block was updated successfully
                if (oldItemBlockNode.Update(documentNode, itemBlock, change, cancellationToken)) continue;

                // else, remove the old node and create a new one
                var newItemBlockNode = CreateBlockNode(documentNode, itemBlock, change, cancellationToken);
                proxy[itemIndex] = newItemBlockNode;
                Grid.SetRow(newItemBlockNode.Control, i);
                Grid.SetColumn(newItemBlockNode.Control, 1);
            }
            else
            {
                var newItemBlockNode = CreateBlockNode(documentNode, itemBlock, change, cancellationToken);
                proxy.Add(newItemBlockNode);
                Grid.SetRow(newItemBlockNode.Control, i);
                Grid.SetColumn(newItemBlockNode.Control, 1);
            }
        }

        while (proxy.Count > listBlock.Count * 2)
        {
            cancellationToken.ThrowIfCancellationRequested();
            proxy.RemoveAt(proxy.Count - 1);
        }

        while (grid.RowDefinitions.Count > listBlock.Count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
        }

        return true;
    }
}