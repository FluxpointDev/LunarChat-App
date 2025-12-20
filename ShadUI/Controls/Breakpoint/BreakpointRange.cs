// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides a dictionary of breakpoints and their corresponding range functions.
/// </summary>
public static class BreakpointRange
{
    /// <summary>
    ///     Gets the dictionary of breakpoints and their corresponding range functions.
    /// </summary>
    public static readonly Dictionary<Breakpoint, Func<double, bool>> Ranges =
        new()
        {
            { Breakpoint.Sm, width => width <= 640 },
            { Breakpoint.SmAndUp, width => width >= 640 },
            { Breakpoint.MdAndDown, width => width < 768 },
            { Breakpoint.Md, width => width is >= 768 and < 1024 },
            { Breakpoint.MdAndUp, width => width > 768 },
            { Breakpoint.LgAndDown, width => width < 1024 },
            { Breakpoint.Lg, width => width is >= 1024 and < 1280 },
            { Breakpoint.LgAndUp, width => width >= 1024 },
            { Breakpoint.XlAndDown, width => width < 1280 },
            { Breakpoint.Xl, width => width is >= 1280 and < 1536 },
            { Breakpoint.XlAndUp, width => width >= 1280 },
            { Breakpoint.Xxl, width => width >= 1536 }
        };
}