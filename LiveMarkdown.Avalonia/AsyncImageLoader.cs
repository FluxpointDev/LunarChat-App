using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg;
using Avalonia.Threading;
using Svg.Model;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// Asynchronously loads images from a given source URL and caches them.
/// Supports both SVG and bitmap images.
/// </summary>
public class AsyncImageLoader
{
    /// <summary>
    /// Attached property for the image source URL.
    /// </summary>
    public static readonly AttachedProperty<string?> SourceProperty =
        AvaloniaProperty.RegisterAttached<AsyncImageLoader, Image, string?>("Source");

    /// <summary>
    /// Sets the source URL for the image.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetSource(Image obj, string? value) => obj.SetValue(SourceProperty, value);

    /// <summary>
    /// Gets the source URL for the image.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string? GetSource(Image obj) => obj.GetValue(SourceProperty);

    /// <summary>
    /// Attached property for the SizeToContent behavior.
    /// </summary>
    public static readonly AttachedProperty<SizeToContent> SizeToContentProperty =
        AvaloniaProperty.RegisterAttached<AsyncImageLoader, Image, SizeToContent>("SizeToContent", SizeToContent.WidthAndHeight);

    /// <summary>
    /// Sets the SizeToContent behavior for the image.
    /// Indicates whether the image should size itself to its content by setting Width and Height.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetSizeToContent(Image obj, SizeToContent value) => obj.SetValue(SizeToContentProperty, value);

    /// <summary>
    /// Gets the SizeToContent behavior for the image.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static SizeToContent GetSizeToContent(Image obj) => obj.GetValue(SizeToContentProperty);

    /// <summary>
    /// Attached property for the SVG CSS styles.
    /// This only works before the image is loaded.
    /// </summary>
    public static readonly AttachedProperty<string?> SvgCssProperty =
        AvaloniaProperty.RegisterAttached<AsyncImageLoader, Image, string?>("SvgCss");

    /// <summary>
    /// Sets the CSS styles for the SVG image.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetSvgCss(Image obj, string? value) => obj.SetValue(SvgCssProperty, value);

    /// <summary>
    /// Gets the CSS styles for the SVG image.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string? GetSvgCss(Image obj) => obj.GetValue(SvgCssProperty);

    /// <summary>
    /// Attached property for the image cache.
    /// </summary>
    public static readonly AttachedProperty<AsyncImageLoaderCache?> CacheProperty =
        AvaloniaProperty.RegisterAttached<AsyncImageLoader, Image, AsyncImageLoaderCache?>("Cache", RamBasedAsyncImageLoaderCache.Shared);

    /// <summary>
    /// Sets the cache for the image loader.
    /// You can use one of the built-in caches (convert from string), or implement your own.
    /// built-in caches include: `None`, `Ram`. Default is `Ram`.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetCache(Image obj, AsyncImageLoaderCache? value) => obj.SetValue(CacheProperty, value);

    /// <summary>
    /// Gets the cache for the image loader.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static AsyncImageLoaderCache? GetCache(Image obj) => obj.GetValue(CacheProperty);

    /// <summary>
    /// Attached property for custom image loaders.
    /// </summary>
    public static readonly AttachedProperty<IReadOnlyCollection<AsyncImageLoaderHandler>> HandlersProperty =
        AvaloniaProperty.RegisterAttached<AsyncImageLoader, Image, IReadOnlyCollection<AsyncImageLoaderHandler>>(
            "Handlers",
            [
                HttpAsyncImageLoaderHandler.Shared,
                LocalFileAsyncImageLoaderHandler.Shared,
                AvaloniaResourceAsyncImageLoaderHandler.Shared
            ]);

    /// <summary>
    /// Sets the custom image loader handlers.
    /// You can add your own handlers to support custom URI schemes.
    /// Default is `Http`, `LocalFile`, `AvaloniaResource`. (HTTP supports redirects and HTTPS)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetHandlers(Image obj, IReadOnlyCollection<AsyncImageLoaderHandler> value) => obj.SetValue(HandlersProperty, value);

    /// <summary>
    /// Gets the custom image loader handlers.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IReadOnlyCollection<AsyncImageLoaderHandler> GetHandlers(Image obj) => obj.GetValue(HandlersProperty);

    private readonly static Dictionary<Image, (Task task, CancellationTokenSource cts)> ImageLoadTasks = new();

    static AsyncImageLoader()
    {
        SourceProperty.Changed.AddClassHandler<Image>(HandleSourceChanged);
    }

    private static void HandleSourceChanged(Image sender, AvaloniaPropertyChangedEventArgs args)
    {
        // This method is always called on the UI thread, so we can safely access the UI elements.

        if (ImageLoadTasks.TryGetValue(sender, out var pair))
        {
            pair.cts.Cancel(); // Cancel the previous loading task if it exists
            ImageLoadTasks.Remove(sender);
        }

        var newSource = args.NewValue as string;
        if (!Uri.TryCreate(newSource, UriKind.RelativeOrAbsolute, out var uri))
        {
            sender.Source = null; // Clear the image source if the new value is not a valid URI
            ApplySizeToContent(sender, null);
            return;
        }

        var cache = sender.GetValue(CacheProperty);
        if (cache?.GetImage(uri) is { } cachedImage)
        {
            sender.Source = cachedImage; // Use the cached image if available
            ApplySizeToContent(sender, cachedImage);
            return;
        }

        var handler = GetHandlers(sender).FirstOrDefault(h => h.SupportedSchemes.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase));
        if (handler is null)
        {
            sender.Source = null; // No handler found for the URI scheme
            ApplySizeToContent(sender, null);
            return;
        }

        var css = sender.GetValue(SvgCssProperty);
        if (string.IsNullOrEmpty(css))
        {
            var fontSize = sender.GetValue(TextElement.FontSizeProperty);
            var fontFamily = sender.GetValue(TextElement.FontFamilyProperty).ToString();
            if (fontFamily == "$Default") fontFamily = "Arial"; // Fallback to Arial if the default is not set
            var color = sender.GetValue(TextElement.ForegroundProperty) switch
            {
                SolidColorBrush solidColorBrush => solidColorBrush.Color,
                _ => Colors.White // Default color if not set
            };
            css = $":nth-child(0) {{ font-size: {fontSize}px; font-family: {fontFamily}; color: #{color.R:X2}{color.G:X2}{color.B:X2}; }}";
        }

        var newPair = CreateLoadPair(sender, uri, css, handler, cache);
        ImageLoadTasks.Add(sender, newPair);
    }

    private static void ApplySizeToContent(Image image, IImage? loadedImage)
    {
        var sizeToContent = GetSizeToContent(image);
        if (sizeToContent == SizeToContent.Manual) return;

        var (imageWidth, imageHeight) = loadedImage switch
        {
            Bitmap bitmap => (bitmap.PixelSize.Width, bitmap.PixelSize.Height),
            SvgImage svgImage => (svgImage.Size.Width, svgImage.Size.Height),
            _ => (double.NaN, double.NaN)
        };

#if NETSTANDARD2_0
        if (double.IsInfinity(imageWidth) || double.IsNaN(imageWidth) || imageWidth <= 1d ||
            double.IsInfinity(imageHeight) || double.IsNaN(imageHeight) || imageHeight <= 1d)
#else
        if (double.IsSubnormal(imageWidth) || imageWidth <= 1d || double.IsSubnormal(imageHeight) || imageHeight <= 1d)
#endif
        {
            image.Width = double.NaN;
            image.Height = double.NaN;
            return;
        }

        var actualWidth = Math.Min(imageWidth, image.MaxWidth);
        var actualHeight = Math.Min(imageHeight, image.MaxHeight);

        // Ensure aspect ratio is maintained
        var widthRatio = actualWidth / imageWidth;
        var heightRatio = actualHeight / imageHeight;
        var minRatio = Math.Min(widthRatio, heightRatio);
        actualWidth = imageWidth * minRatio;
        actualHeight = imageHeight * minRatio;

        image.Width = sizeToContent.HasFlag(SizeToContent.Width) ? actualWidth : double.NaN;
        image.Height = sizeToContent.HasFlag(SizeToContent.Height) ? actualHeight : double.NaN;
    }

    private static (Task task, CancellationTokenSource cts) CreateLoadPair(
        Image image,
        Uri uri,
        string? css,
        AsyncImageLoaderHandler handler,
        AsyncImageLoaderCache? cache)
    {
        var cts = new CancellationTokenSource();
        var task = Task.Run(
                async () =>
                {
                    try
                    {
                        // check if the stream is svg
                        var buffer = new byte[16];
#if NETSTANDARD2_0
                        using var stream = await handler.LoadAsync(uri, cts.Token);
                        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
#else
                        await using var stream = await handler.LoadAsync(uri, cts.Token);
                        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token);
#endif
                        if (bytesRead == 0)
                        {
                            return null;
                        }

                        stream.Seek(0, SeekOrigin.Begin); // Reset the stream position

                        var isBinary = buffer.Take(bytesRead).Any(b => b == 0); // check for null bytes, which indicate binary data
                        if (isBinary)
                        {
                            // If the stream is binary, treat it as a Bitmap
                            return (object)WriteableBitmap.Decode(stream);
                        }

                        return SvgSource.Load(stream, new SvgParameters { Css = css });
                    }
                    catch (OperationCanceledException)
                    {
                        // Task was cancelled, do nothing
                        throw;
                    }
                    catch
                    {
                        // Handle other exceptions as needed
                        return null; // Clear the image source on error
                    }
                },
                cts.Token)
            .ContinueWith(
                t =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        if (ImageLoadTasks.TryGetValue(image, out var pair) && pair.cts == cts)
                        {
                            ImageLoadTasks.Remove(image); // Remove the task from the dictionary
                        }

                        if (t.Exception is not null) return; // Operation was cancelled or failed, do nothing

                        IImage? result = t.Result switch
                        {
                            Bitmap bitmap => bitmap,
                            SvgSource svgSource => new SvgImage { Source = svgSource },
                            _ => null // Clear the image source if the result is not a valid image
                        };

                        if (result is not null) cache?.SetImage(uri, result);

                        image.Source = result;
                        ApplySizeToContent(image, result);
                    });
                },
                cts.Token);

        return (task, cts);
    }
}

#if NETSTANDARD2_0
public class AsyncImageLoaderCacheTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
    {
        if (value is string str)
        {
            return str switch
            {
                "None" => null,
                "Ram" => RamBasedAsyncImageLoaderCache.Shared,
                _ => throw new NotSupportedException($"Cache type '{str}' is not supported.")
            };
        }

        return base.ConvertFrom(context, culture, value);
    }
}
#else
public class AsyncImageLoaderCacheTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            return str switch
            {
                "None" => null,
                "Ram" => RamBasedAsyncImageLoaderCache.Shared,
                _ => throw new NotSupportedException($"Cache type '{str}' is not supported.")
            };
        }

        return base.ConvertFrom(context, culture, value);
    }
}
#endif

/// <summary>
/// Handler for loading images from specific URI schemes.
/// </summary>
public abstract class AsyncImageLoaderHandler
{
    /// <summary>
    /// Supported URI schemes (e.g., "http", "https", "file").
    /// </summary>
    public abstract IEnumerable<string> SupportedSchemes { get; }

    public abstract Task<Stream> LoadAsync(Uri uri, CancellationToken cancellationToken);
}

/// <summary>
/// Handler for loading images over HTTP and HTTPS.
/// Supports redirects.
/// </summary>
/// <param name="httpClient"></param>
public class HttpAsyncImageLoaderHandler(HttpClient httpClient) : AsyncImageLoaderHandler
{
    public static HttpAsyncImageLoaderHandler Shared { get; } = new();

    public override IEnumerable<string> SupportedSchemes => ["http", "https"];

    public HttpAsyncImageLoaderHandler() : this(new HttpClient(new HttpClientHandler { AllowAutoRedirect = true })) { }

    public override async Task<Stream> LoadAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (uri.Scheme != "http" && uri.Scheme != "https") throw new NotSupportedException("Only HTTP and HTTPS URIs are supported.");

        var response = await httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
#if NETSTANDARD2_0
        return await response.Content.ReadAsStreamAsync();
#else
        return await response.Content.ReadAsStreamAsync(cancellationToken);
#endif
    }
}

/// <summary>
/// Handler for loading images from local file URIs.
/// </summary>
public class LocalFileAsyncImageLoaderHandler : AsyncImageLoaderHandler
{
    public static LocalFileAsyncImageLoaderHandler Shared { get; } = new();

    public override IEnumerable<string> SupportedSchemes => ["file"];

    public override Task<Stream> LoadAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (!uri.IsFile) throw new NotSupportedException("Only file URIs are supported.");

        Stream stream = File.OpenRead(uri.LocalPath);
        return Task.FromResult(stream);
    }
}

public class AvaloniaResourceAsyncImageLoaderHandler : AsyncImageLoaderHandler
{
    public static AvaloniaResourceAsyncImageLoaderHandler Shared { get; } = new();

    public override IEnumerable<string> SupportedSchemes => ["avares"];

    public override Task<Stream> LoadAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (uri.Scheme != "avares") throw new NotSupportedException("Only avares URIs are supported.");

        return Task.FromResult(AssetLoader.Open(uri));
    }
}

[TypeConverter(typeof(AsyncImageLoaderCacheTypeConverter))]
public abstract class AsyncImageLoaderCache
{
    public abstract IImage? GetImage(Uri uri);

    public abstract void SetImage(Uri uri, IImage image);
}

public class RamBasedAsyncImageLoaderCache : AsyncImageLoaderCache
{
    public static RamBasedAsyncImageLoaderCache Shared { get; } = new();

    private readonly Dictionary<Uri, WeakReference<IImage>> _cache = new();

    private int _checkThreshold = 16;

    public override IImage? GetImage(Uri uri)
    {
        lock (_cache)
        {
            if (_cache.TryGetValue(uri, out var weakRef) && weakRef.TryGetTarget(out var image))
            {
                return image;
            }

            return null;
        }
    }

    public override void SetImage(Uri uri, IImage image)
    {
        lock (_cache)
        {
            _cache[uri] = new WeakReference<IImage>(image);

            if (_cache.Count <= _checkThreshold) return;

            // Clean up weak references that are no longer alive
            var keysToRemove = _cache.Where(kvp => !kvp.Value.TryGetTarget(out _)).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }

            if (_cache.Count > _checkThreshold) _checkThreshold *= 2;
            else if (_cache.Count < _checkThreshold / 4) _checkThreshold /= 2;
        }
    }
}