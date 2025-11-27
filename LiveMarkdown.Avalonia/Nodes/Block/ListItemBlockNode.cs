using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public sealed class ListItemBlockNode : ContainerBlockNode<ListItemBlock>
{
    public ListItemBlockNode()
    {
        container.Classes.Add("ListItemBlock");
    }
}