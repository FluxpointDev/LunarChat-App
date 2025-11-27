using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public class CodeBlockNode : BlockNode<Markdig.Syntax.CodeBlock>
{
    protected override MarkdownTextBlock? TextBlock => _codeBlock.CodeTextBlock;

    public override Control Control { get; }

    private readonly CodeBlock _codeBlock;

    public CodeBlockNode()
    {
        Control = _codeBlock = new CodeBlock
        {
            Classes = { "CodeBlock" },
            AutoSyntaxHighlight = false
        };
        _codeBlock.ApplyTemplate(); // Ensure the template is applied to initialize the CodeTextBlock
    }

    // ReSharper disable once ConvertTypeCheckPatternToNullCheck
    protected override bool MatchesBlock(Markdig.Syntax.CodeBlock block) => block is Markdig.Syntax.CodeBlock or FencedCodeBlock;

    protected override bool UpdateCore(
        DocumentNode documentNode,
        Markdig.Syntax.CodeBlock codeBlock,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (codeBlock.Lines.Lines is null) return false;

        var inlines = _codeBlock.Inlines;
        foreach (var (slice, lineIndex) in codeBlock.Lines.Lines.Take(codeBlock.Lines.Count).Select((l, i) => (l.Slice, i)))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var inlineIndex = lineIndex * 2;

            // Skip if the slice is completely outside the change range
            if (inlines.Count > inlineIndex &&
                (slice.End < change.StartIndex || change.StartIndex + change.Length <= slice.Start)) continue;

            if (inlines.Count <= inlineIndex)
            {
                if (inlines.Count % 2 == 1)
                {
                    // we need to add a LineBreak before the new Run
                    inlines.Add(new LineBreak());
                }

                inlines.Add(new Run(slice.ToString()));
            }
            else if (inlines[inlineIndex] is Run run)
            {
                // Update existing run
                run.Text = slice.ToString();
                run.Classes.Remove(SyntaxHighlighting.FormattedClassName);
            }
            else
            {
                // Replace it with a new run if it's not a Run
                inlines[inlineIndex] = new Run(slice.ToString());
            }

            if (lineIndex < codeBlock.Lines.Count - 1)
            {
                // Add a line break after each line except the last one
                if (inlines.Count <= inlineIndex + 1)
                {
                    inlines.Add(new LineBreak());
                }
                else if (inlines[inlineIndex + 1] is not LineBreak)
                {
                    // Replace it with a LineBreak if it's not a LineBreak
                    inlines[inlineIndex + 1] = new LineBreak();
                }
            }
        }

        while (inlines.Count > codeBlock.Lines.Count * 2 - 1)
        {
            // Remove excess inlines
            inlines.RemoveAt(inlines.Count - 1);
        }

        // Highlighting only works for closed FencedCodeBlock with Info
        if (codeBlock is not FencedCodeBlock fencedCodeBlock) return true;

        _codeBlock.Language = fencedCodeBlock.Info?.Trim();
        _codeBlock.HighlightSyntax();

        return true;
    }
}