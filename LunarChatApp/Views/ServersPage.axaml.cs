using Avalonia.Controls;
using LiveMarkdown.Avalonia;
using LunarChatApp.Services;

namespace LunarChatApp;

[Page("servers")]
public partial class ServersPage : UserControl
{
    public ServersPage()
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

    public ServerData? SelectedServer;

    private void OpenSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
}