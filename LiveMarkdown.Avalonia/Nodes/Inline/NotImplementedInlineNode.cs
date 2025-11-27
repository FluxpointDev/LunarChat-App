using Avalonia.Controls.Documents;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// A node that represents an inline that is not yet implemented.
/// </summary>
public class NotImplementedInlineNode(Type markdownType) : InlineNode
{
    public override Inline Inline { get; } = new Run
    {
        Classes = { "NotImplementedInline" }
    };

    protected override bool UpdateCore(
        DocumentNode documentNode,
        MarkdownObject markdownObject,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        return documentNode.GetType() == markdownType;
    }
}