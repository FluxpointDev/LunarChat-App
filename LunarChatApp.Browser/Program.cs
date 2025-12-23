using Avalonia;
using Avalonia.Browser;
using Avalonia.Media;
using LunarChatApp;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        JSHost.ImportAsync("JSRuntime", "/test.js");
        return BuildAvaloniaApp()
                .WithInterFont()
                .With(new FontManagerOptions
                {
                    DefaultFamilyName = "avares://Avalonia.Fonts.Inter/Assets#Inter"
                })
                .StartBrowserAppAsync("out");
    }


    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}