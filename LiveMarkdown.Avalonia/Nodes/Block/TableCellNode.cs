using Avalonia.Controls;
using Markdig.Extensions.Tables;

namespace LiveMarkdown.Avalonia;

public sealed class TableCellNode : ContainerBlockNode<TableCell>
{
    public override Control Control { get; }

    public TableCellNode()
    {
        Control = new Border
        {
            Classes = { "TableCell" },
            Child = container
        };
    }
}