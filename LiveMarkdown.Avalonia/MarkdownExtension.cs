using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public static class MarkdownExtension
{
    public static MarkdownPipelineBuilder UseCodeBlockSpanFixer(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.ReplaceOrAdd<CodeBlockSpanFixerExtension>(new CodeBlockSpanFixerExtension());
        return pipeline;
    }
}

/// <summary>
/// Markdown extension that fixes the spans of code blocks.
/// </summary>
public class CodeBlockSpanFixerExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var index = pipeline.BlockParsers.FindIndex(x => x is FencedCodeBlockParser);
        if (index == -1) pipeline.BlockParsers.Add(new CodeBlockSpanFixerParser());
        else pipeline.BlockParsers[index] = new CodeBlockSpanFixerParser();
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }
}

/// <summary>
/// A parser that fixes the spans of code blocks.
/// </summary>
public class CodeBlockSpanFixerParser : FencedCodeBlockParser
{
    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        var state = base.TryContinue(processor, block);
        var currentBlock = block;
        while (currentBlock is not null)
        {
            FixSpan(ref currentBlock.Span, processor);
            currentBlock = currentBlock.Parent;
        }
        return state;
    }

    private static void FixSpan(ref SourceSpan span, BlockProcessor processor)
    {
        span = new SourceSpan(
            Math.Min(span.Start, processor.Line.Start),
            Math.Max(span.End, processor.Line.End));
    }
}