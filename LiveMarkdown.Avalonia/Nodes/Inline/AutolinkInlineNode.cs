using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Inline = Avalonia.Controls.Documents.Inline;

namespace LiveMarkdown.Avalonia;

public sealed class AutolinkInlineNode : InlineNode<AutolinkInline>
{
    public override Inline Inline { get; }

    private readonly InlineHyperlink inlineHyperlink;

    public AutolinkInlineNode()
    {
        Inline = inlineHyperlink = new InlineHyperlink
        {
            Classes = { "AutoLink" }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        AutolinkInline autolink,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        Uri.TryCreate(autolink.Url, UriKind.RelativeOrAbsolute, out var uri);
        inlineHyperlink.HRef = uri;

        if (inlineHyperlink.Inlines is [Run run]) run.Text = autolink.Url;
        else
        {
            inlineHyperlink.Inlines.Clear();
            inlineHyperlink.Inlines.Add(
                new Run
                {
                    Classes = { "Autolink" },
                    Text = autolink.Url
                });
        }

        return true;
    }
}