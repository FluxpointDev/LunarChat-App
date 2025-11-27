namespace LiveMarkdown.Avalonia;

/// <summary>
/// A factory for creating MarkdownNode instances for a specific MarkdownObject type.
/// It implements IComparable to allow sorting based on type compatibility.
/// </summary>
/// <typeparam name="TNode"></typeparam>
public class MarkdownNodeFactory<TNode> : IMarkdownNodeFactory<TNode>, IComparable where TNode : MarkdownNode, new()
{
    public Type MarkdownType { get; } =
        typeof(TNode).BaseType?.GetGenericArguments()[0] ??
        throw new InvalidOperationException($"Cannot determine MarkdownType for {typeof(TNode).FullName}");

    public TNode CreateNode() => new();

    public int CompareTo(object? other)
    {
        if (other is not IMarkdownNodeFactory otherFactory) return 1;
        if (ReferenceEquals(this, other)) return 0;
        if (MarkdownType == otherFactory.MarkdownType) return 0;
        if (MarkdownType.IsAssignableFrom(otherFactory.MarkdownType)) return 1;
        if (otherFactory.MarkdownType.IsAssignableFrom(MarkdownType)) return -1;
        return MarkdownType.FullName?.CompareTo(otherFactory.MarkdownType.FullName) ?? -1;
    }

    public override bool Equals(object? obj) => CompareTo(obj) == 0;

    public override int GetHashCode() => MarkdownType.GetHashCode();
}