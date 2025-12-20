using Avalonia.Rendering.Composition;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Usable extension methods for making an element scrollable.
/// </summary>
internal static class ScrollableExt
{
    /// <summary>
    ///     Makes the visual scrollable.
    /// </summary>
    /// <param name="compositionVisual"></param>
    public static void MakeScrollable(this CompositionVisual? compositionVisual)
    {
        if (compositionVisual == null) return;

        var compositor = compositionVisual.Compositor;

        var animationGroup = compositor.CreateAnimationGroup();
        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";

        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(250);

        var implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        animationGroup.Add(offsetAnimation);
        implicitAnimationCollection["Offset"] = animationGroup;
        compositionVisual.ImplicitAnimations = implicitAnimationCollection;
    }
}