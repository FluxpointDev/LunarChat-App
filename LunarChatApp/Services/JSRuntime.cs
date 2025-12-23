using System.Runtime.InteropServices.JavaScript;

namespace LunarChatApp.Services;

internal static partial class JSRuntime
{
    [JSImport("playSound", "JSRuntime")]
    public static partial void PlaySound(string name);

    [JSImport("stopSound", "JSRuntime")]
    public static partial void StopSound();
}