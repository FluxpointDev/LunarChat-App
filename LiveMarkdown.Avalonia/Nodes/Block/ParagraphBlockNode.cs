using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public sealed class ParagraphBlockNode : InlineCollectionNode<ParagraphBlock>
{
    public ParagraphBlockNode()
    {
        Control.Classes.Add("ParagraphBlock");
    }
}