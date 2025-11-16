using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;

namespace LunarChatApp.Host;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
        var app = builder.Build();
        app.UseHttpsRedirection();
        var extensionProvider = new FileExtensionContentTypeProvider();
        extensionProvider.Mappings.Add(".dat", "application/octet-stream");
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = extensionProvider
        });
        app.MapStaticAssets();
        app.UseForwardedHeaders();
        app.UseResponseCompression();
        app.MapFallbackToFile("index.html");
        app.Run();
    }
}
