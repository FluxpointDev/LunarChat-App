using Avalonia.Controls;
using LiveMarkdown.Avalonia;

namespace LunarChatApp;

public partial class TestComponents : UserControl
{
    public TestComponents()
    {
        InitializeComponent();

        var markdownBuilder = new ObservableStringBuilder();
        MarkdownRenderer.MarkdownBuilder = markdownBuilder;
        // Append Markdown content, this will trigger re-rendering
        markdownBuilder.Append("# Hello, Markdown!\n" +
            "https://google.com\n" +
            "- Test 1\n" +
            "- Test 2\n" +
            "**Bold**\n" +
            "```cs\n" +
            "public void Test() {\n" +
            "}\n" +
            "```");
    }
}