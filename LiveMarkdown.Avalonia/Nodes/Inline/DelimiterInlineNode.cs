using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Inline = Avalonia.Controls.Documents.Inline;

namespace LiveMarkdown.Avalonia;

public class DelimiterInlineNode : InlineNode<DelimiterInline>
{
    public override Inline Inline { get; }

    private readonly Run run;

    public DelimiterInlineNode()
    {
        Inline = run = new Run
        {
            Classes = { "Delimiter" }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        DelimiterInline delimiter,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        run.Text = delimiter.ToLiteral();
        return true;
    }
}