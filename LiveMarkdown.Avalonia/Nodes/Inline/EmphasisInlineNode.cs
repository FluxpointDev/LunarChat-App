using Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax.Inlines;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// A node that represents an emphasis inline (bold, italic, etc.).
/// </summary>
public class EmphasisInlineNode : ContainerInlineNode<EmphasisInline>
{
    protected override bool UpdateCore(
        DocumentNode documentNode,
        EmphasisInline emphasisInline,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        var span = (Span)Inline;
        switch (emphasisInline.DelimiterChar)
        {
            case '*' when emphasisInline.DelimiterCount == 2: // bold
            case '_' when emphasisInline.DelimiterCount == 2: // bold
                span.FontWeight = FontWeight.Bold;
                break;
            case '*': // italic
            case '_': // italic
                span.FontStyle = FontStyle.Italic;
                break;
            case '~': // 2x strike through, 1x subscript
                if (emphasisInline.DelimiterCount == 2)
                    span.TextDecorations = TextDecorations.Strikethrough;
                else
                    span.BaselineAlignment = BaselineAlignment.Subscript;
                break;
            case '^': // 1x superscript
                span.BaselineAlignment = BaselineAlignment.Superscript;
                break;
            case '+': // 2x underline
                span.TextDecorations = TextDecorations.Underline;
                break;
            case '=': // 2x Marked
                // documentNode: Implement Marked
                break;
        }

        return base.UpdateCore(documentNode, emphasisInline, in change, cancellationToken);
    }
}