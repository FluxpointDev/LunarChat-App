using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Inline = Avalonia.Controls.Documents.Inline;

namespace LiveMarkdown.Avalonia;

public class HtmlEntityInlineNode : InlineNode<HtmlEntityInline>
{
    public override Inline Inline { get; }

    private readonly Run run;

    public HtmlEntityInlineNode()
    {
        Inline = run = new Run
        {
            Classes = { "HtmlEntity" }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        HtmlEntityInline htmlEntity,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        run.Text = htmlEntity.Transcoded.ToString();
        return true;
    }
}