namespace LiveMarkdown.Avalonia;

public interface IMarkdownNodeFactory
{
    Type MarkdownType { get; }
}

public interface IMarkdownNodeFactory<out TNode> : IMarkdownNodeFactory where TNode : MarkdownNode
{
    TNode CreateNode();
}
