using Avalonia.Controls;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public sealed class QuoteBlockNode : ContainerBlockNode<QuoteBlock>
{
    public override Control Control { get; }

    public QuoteBlockNode()
    {
        Control = new Border
        {
            Classes = { "QuoteBlock" },
            Child = container
        };
    }
}