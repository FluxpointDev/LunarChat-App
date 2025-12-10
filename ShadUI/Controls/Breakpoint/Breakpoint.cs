// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents the different breakpoints for responsive design.
/// </summary>
public enum Breakpoint
{
    /// <summary>
    ///     Small breakpoint (up to 640px).
    /// </summary>
    Sm,

    /// <summary>
    ///     Small and up breakpoint (640px and above).
    /// </summary>
    SmAndUp,

    /// <summary>
    ///     Medium and down breakpoint (less than 768px).
    /// </summary>
    MdAndDown,

    /// <summary>
    ///     Medium breakpoint (768px to 1024px).
    /// </summary>
    Md,

    /// <summary>
    ///     Medium and up breakpoint (768px and above).
    /// </summary>
    MdAndUp,

    /// <summary>
    ///     Large and down breakpoint (less than 1024px).
    /// </summary>
    LgAndDown,

    /// <summary>
    ///     Large breakpoint (1024px to 1280px).
    /// </summary>
    Lg,

    /// <summary>
    ///     Large and up breakpoint (1024px and above).
    /// </summary>
    LgAndUp,

    /// <summary>
    ///     Extra large and down breakpoint (less than 1280px).
    /// </summary>
    XlAndDown,

    /// <summary>
    ///     Extra large breakpoint (1280px to 1536px).
    /// </summary>
    Xl,

    /// <summary>
    ///     Extra large and up breakpoint (1280px and above).
    /// </summary>
    XlAndUp,

    /// <summary>
    ///     Extra, extra large breakpoint (1536px and above).
    /// </summary>
    Xxl
}