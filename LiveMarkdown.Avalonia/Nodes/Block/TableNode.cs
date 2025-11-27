using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Markdig.Extensions.Tables;

namespace LiveMarkdown.Avalonia;

public class TableNode : BlockNode<Table>
{
    public override Control Control { get; }

    private readonly Grid container;
    private readonly MarkdownRenderer.BlocksProxy proxy;

    public TableNode()
    {
        container = new Grid();
        proxy = new MarkdownRenderer.BlocksProxy(container.Children);
        Control = new ScrollViewer
        {
            Classes = { "Table" },
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = new Border
            {
                Classes = { "Table" },
                Child = container
            }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        Table table,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        if (table.ColumnDefinitions.Count == 0) return false;

        while (table.ColumnDefinitions.Count < container.ColumnDefinitions.Count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            container.ColumnDefinitions.RemoveAt(container.ColumnDefinitions.Count - 1);
        }
        while (table.ColumnDefinitions.Count > container.ColumnDefinitions.Count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        var rowIndex = 0;
        var cellIndex = 0;
        foreach (var row in table.OfType<TableRow>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (rowIndex >= container.RowDefinitions.Count)
            {
                container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            foreach (var (cell, columnIndex) in row.OfType<TableCell>().Select((c, i) => (c, i)))
            {
                cancellationToken.ThrowIfCancellationRequested();

                Control cellControl;
                do
                {
                    if (proxy.Count > cellIndex)
                    {
                        // existing item block node, update it
                        var oldCellBlockNode = proxy[cellIndex];
                        cellControl = oldCellBlockNode.Control;

                        // if Update returned true; it means the block was updated successfully
                        if (oldCellBlockNode.Update(documentNode, cell, change, cancellationToken)) break;

                        // else, remove the old node and create a new one
                        var newCellBlockNode = CreateBlockNode(documentNode, cell, change, cancellationToken);
                        proxy[cellIndex] = newCellBlockNode;
                        cellControl = newCellBlockNode.Control;
                    }
                    else
                    {
                        var newCellBlockNode = CreateBlockNode(documentNode, cell, change, cancellationToken);
                        proxy.Add(newCellBlockNode);
                        cellControl = newCellBlockNode.Control;
                    }
                }
                while (false);

                cellIndex++;
                Grid.SetRow(cellControl, rowIndex);
                Grid.SetColumn(cellControl, columnIndex);

                if (row.IsHeader)
                {
                    if (!cellControl.Classes.Contains("Header"))
                    {
                        cellControl.Classes.Add("Header");
                    }
                }
                else
                {
                    cellControl.Classes.Remove("Header");
                }

                if (columnIndex >= table.ColumnDefinitions.Count) continue;
                if (cellControl is not Border { Child: { } child }) continue;
                var columnDefinition = table.ColumnDefinitions[columnIndex];
                child.HorizontalAlignment = columnDefinition.Alignment switch
                {
                    TableColumnAlign.Left => HorizontalAlignment.Left,
                    TableColumnAlign.Center => HorizontalAlignment.Center,
                    TableColumnAlign.Right => HorizontalAlignment.Right,
                    _ => HorizontalAlignment.Stretch
                };
            }

            rowIndex++;
        }

        var columnCount = table.ColumnDefinitions.Count;
        var cellCount = rowIndex * columnCount;
        while (proxy.Count > cellCount)
        {
            cancellationToken.ThrowIfCancellationRequested();
            proxy.RemoveAt(proxy.Count - 1);
        }

        if (rowIndex == 0 || columnCount == 0)
        {
            return false;
        }

        while (rowIndex < container.RowDefinitions.Count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            container.RowDefinitions.RemoveAt(container.RowDefinitions.Count - 1);
        }

        return true;
    }
}