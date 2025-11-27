using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Inline = Avalonia.Controls.Documents.Inline;

namespace LiveMarkdown.Avalonia;

public class LiteralInlineNode : InlineNode<LiteralInline>
{
    public override Inline Inline { get; }

    private readonly Run run;

    public LiteralInlineNode()
    {
        Inline = run = new Run
        {
            Classes = { "Literal" }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        LiteralInline literal,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        run.Text = literal.Content.ToString();
        return true;
    }
}