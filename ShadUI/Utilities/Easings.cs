using Avalonia.Animation.Easings;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Defines the intensity levels for easing animations.
/// </summary>
public enum EasingIntensity
{
    /// <summary>
    ///     Soft intensity with subtle bounce effect.
    /// </summary>
    Soft,

    /// <summary>
    ///     Normal intensity with balanced bounce effect.
    /// </summary>
    Normal,

    /// <summary>
    ///     Strong intensity with pronounced bounce effect.
    /// </summary>
    Strong
}

/// <summary>
///     Provides an easing function that combines ease-in-back and ease-out-back effects.
///     This creates a smooth animation that starts with a back effect, transitions through the middle,
///     and ends with another back effect.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
public class EaseInBackOutBack : Easing
{
    /// <summary>
    ///     Gets or sets the bounce intensity for the easing animation.
    /// </summary>
    public EasingIntensity BounceIntensity { get; set; } = EasingIntensity.Normal;

    /// <summary>
    ///     Applies the ease-in-back-out-back function to the given progress value.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    /// <returns>The eased progress value.</returns>
    public override double Ease(double progress)
    {
        var backIntensity = BounceIntensity switch
        {
            EasingIntensity.Soft => 0.9,
            EasingIntensity.Normal => 1.15,
            EasingIntensity.Strong => 1.5,
            _ => 1.0
        };

        var backOvershoot = backIntensity * 1.525;

        if (progress < 0.5)
        {
            var normalizedProgress = 2 * progress;
            return Math.Pow(normalizedProgress, 2) * ((backOvershoot + 1) * normalizedProgress - backOvershoot) / 2.0;
        }
        else
        {
            var normalizedProgress = 2 * progress - 2;
            return (Math.Pow(normalizedProgress, 2) * ((backOvershoot + 1) * normalizedProgress + backOvershoot) + 2) /
                   2.0;
        }
    }
}

/// <summary>
///     Provides an easing function that combines ease-in and ease-out back effects.
///     This creates a smooth animation that starts and ends with back effects.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
public class EaseInOutBack : Easing
{
    /// <summary>
    ///     Gets or sets the bounce intensity for the easing animation.
    /// </summary>
    public EasingIntensity BounceIntensity { get; set; } = EasingIntensity.Normal;

    /// <summary>
    ///     Applies the ease-in-out-back function to the given progress value.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    /// <returns>The eased progress value.</returns>
    public override double Ease(double progress)
    {
        var backIntensity = BounceIntensity switch
        {
            EasingIntensity.Soft => 0.9,
            EasingIntensity.Normal => 1.15,
            EasingIntensity.Strong => 1.5,
            _ => 1.0
        };

        var backOvershoot = backIntensity + 1;

        var currentProgress = progress;
        var smoothedProgress = currentProgress * currentProgress * (2 - currentProgress);

        return 1 + backOvershoot * Math.Pow(smoothedProgress - 1, 3) +
               backIntensity * Math.Pow(smoothedProgress - 1, 2);
    }
}

/// <summary>
///     Provides an easing function that creates a back effect at the end of the animation.
///     This creates a smooth animation that ends with a slight overshoot and bounce back.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
public class EaseOutBack : Easing
{
    /// <summary>
    ///     Gets or sets the bounce intensity for the easing animation.
    /// </summary>
    public EasingIntensity BounceIntensity { get; set; } = EasingIntensity.Normal;

    /// <summary>
    ///     Applies the ease-out-back function to the given progress value.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    /// <returns>The eased progress value.</returns>
    public override double Ease(double progress)
    {
        var backIntensity = BounceIntensity switch
        {
            EasingIntensity.Soft => 0.9,
            EasingIntensity.Normal => 1.15,
            EasingIntensity.Strong => 1.5,
            _ => 1.0
        };

        var backOvershoot = backIntensity + 1;

        return 1 + backOvershoot * Math.Pow(progress - 1, 3) + backIntensity * Math.Pow(progress - 1, 2);
    }
}

/// <summary>
///     Provides a custom ease-out function that creates a smooth deceleration effect.
///     This easing function applies a square root transformation to the progress
///     and then applies a cubic ease-out effect.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
public class EaseOut : Easing
{
    /// <summary>
    ///     Applies the custom ease-out function to the given progress value.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    /// <returns>The eased progress value.</returns>
    public override double Ease(double progress)
    {
        var warpedProgress = Math.Sqrt(progress);
        return 1.0 - Math.Pow(1.0 - warpedProgress, 3);
    }
}

/// <summary>
///     Provides a custom ease-in-out function that creates a smooth acceleration and deceleration effect.
///     This easing function applies a square root transformation to the progress
///     and then applies a cubic ease-in-out effect.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
public class EaseInOut : Easing
{
    /// <summary>
    ///     Applies the custom ease-in-out function to the given progress value.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    /// <returns>The eased progress value.</returns>
    public override double Ease(double progress)
    {
        var warpedProgress = Math.Sqrt(progress);

        if (warpedProgress < 0.5)
        {
            return 4.0 * warpedProgress * warpedProgress * warpedProgress;
        }

        var factor = -2.0 * warpedProgress + 2.0;
        return 1.0 - Math.Pow(factor, 3) / 2.0;
    }
}