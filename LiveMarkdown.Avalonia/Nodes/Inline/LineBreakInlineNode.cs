using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Inline = Avalonia.Controls.Documents.Inline;

namespace LiveMarkdown.Avalonia;

public class LineBreakInlineNode : InlineNode<LineBreakInline>
{
    public override Inline Inline { get; } = new LineBreak
    {
        Classes = { "LineBreak" }
    };

    protected override bool UpdateCore(
        DocumentNode documentNode,
        LineBreakInline lineBreak,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        return true;
    }
}