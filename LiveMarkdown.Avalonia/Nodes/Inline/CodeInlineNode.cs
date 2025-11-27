using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Inline = Avalonia.Controls.Documents.Inline;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// A node that represents a code inline.
/// </summary>
public class CodeInlineNode : InlineNode<CodeInline>
{
    protected override MarkdownTextBlock TextBlock => textBlock;

    public override Inline Inline => inlineUIContainer;

    private readonly InlineUIContainer inlineUIContainer;
    private readonly MarkdownTextBlock textBlock;

    public CodeInlineNode()
    {
        inlineUIContainer = new InlineUIContainer
        {
            Classes = { "Code" },
            Child = new Border
            {
                Child = textBlock = new MarkdownTextBlock()
            }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        CodeInline code,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        textBlock.Text = code.Content;
        return true;
    }
}