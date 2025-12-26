using System.Runtime.InteropServices.JavaScript;

namespace LunarChatApp.Services;

internal static partial class JSRuntime
{
    [JSImport("globalThis.document.createElement")]
    public static partial JSObject CreateElement(string tagName);

    [JSImport("playSound", "JSRuntime")]
    public static partial void PlaySound(string name);

    [JSImport("stopSound", "JSRuntime")]
    public static partial void StopSound();

    public static JSObject CreateElement2(string v)
    {
        return CreateElement(v);
    }
}