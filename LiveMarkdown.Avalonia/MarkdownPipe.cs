using Markdig;
using Markdig.Parsers;

namespace LiveMarkdown.Avalonia;

public static class MarkdownPipe
{
    private static readonly MarkdownPipeline pipeline2 = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
        // > [!NOTE]  
        // > Highlights information that users should take into account, even when skimming.
        .UseAlertBlocks()
        .UseListExtras()
        .UseCustomAutoLinks()

        // Broken
        //.UseAbbreviations()
        //.UseCustomContainers()
        //.UseDefinitionLists()


        // Not needed
        //.UseAbbreviations()
        //.UseCitations()
        //.UseFooters()
        //.UseFigures()
        //.UseFootnotes()
        //.UseMathematics()
        //.UseMediaLinks()
        //.UseDiagrams()
        //.UseGenericAttributes()

        // Maybe needed
        //.UseEmphasisExtras()
        //.UseGridTables()
        //.UsePipeTables()
        //.UseTaskLists()

        //.EnableTrackTrivia()
        .DisableSatextHeadings()
        .DisableHtml()
        .UseCodeBlockSpanFixer()
        .Build();

    public static MarkdownPipelineBuilder DisableSatextHeadings(this MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.TryFind<ParagraphBlockParser>(out var parser))
        {
            parser.ParseSetexHeadings = false;
        }
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseCustomAutoLinks(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.ReplaceOrAdd<CustomAutoLinkExtension>(new CustomAutoLinkExtension());
        return pipeline;
    }
}
