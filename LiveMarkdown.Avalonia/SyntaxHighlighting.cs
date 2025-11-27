using Avalonia.Controls.Documents;
using Avalonia.Media;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;
using FontStyle = Avalonia.Media.FontStyle;

namespace LiveMarkdown.Avalonia;

internal record RegistryContext
{
    public static RegistryContext DarkPlus { get; } = new(ThemeName.DarkPlus);

    public RegistryOptions Options { get; }

    public Registry Registry { get; }

    public Theme Theme { get; }

    private readonly Dictionary<Color, SolidColorBrush> _colorBrushCache = new();

    private RegistryContext(ThemeName themeName)
    {
        Options = new RegistryOptions(themeName);
        Registry = new Registry(Options);
        Theme = Registry.GetTheme();
    }

    public SolidColorBrush GetBrush(Color color)
    {
        if (_colorBrushCache.TryGetValue(color, out var brush)) return brush;
        brush = new SolidColorBrush(color);
        _colorBrushCache[color] = brush;
        return brush;
    }
}

/// <summary>
/// Handles syntax highlighting for source code using TextMateSharp
/// and renders it into an Avalonia InlineCollection.
/// </summary>
public class SyntaxHighlighting
{
    public const string FormattedClassName = "formatted";

    public static bool IsRunFormatted(Run run) => run.Classes.Contains(FormattedClassName);

    private readonly static Dictionary<string, WeakReference<SyntaxHighlighting>> Cache = [];

    private readonly IGrammar? _grammar;

    public static SyntaxHighlighting Create(string languageName)
    {
        lock (Cache)
        {
            if (Cache.TryGetValue(languageName, out var weakRef) && weakRef.TryGetTarget(out var cached))
            {
                return cached;
            }
        }

        var instance = new SyntaxHighlighting(languageName);
        lock (Cache) Cache[languageName] = new WeakReference<SyntaxHighlighting>(instance);
        return instance;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxHighlighting"/> class.
    /// </summary>
    /// <param name="languageName"></param>
    private SyntaxHighlighting(string languageName)
    {
        // Initialize the registry with the DarkPlus theme.
        var context = RegistryContext.DarkPlus;

        // Get the scope name from the language name (e.g., "csharp" -> "source.cs") and load the grammar.
        var scopeName = context.Options.GetScopeByLanguageId(languageName) ?? context.Options.GetScopeByExtension('.' + languageName);
        if (scopeName == null) return;

        _grammar = context.Registry.LoadGrammar(scopeName);
    }

    /// <summary>
    /// Formats the source code and populates the InlineCollection with styled runs.
    /// </summary>
    public void FormatInlines(InlineCollection inlines)
    {
        if (_grammar is null) return;

        IStateStack? ruleStack = null;

        // Tokenize each line of the source code.
        for (var i = 0; i < inlines.Count; i++)
        {
            if (inlines[i] is not Run { Text: { } line } run) continue;
            if (IsRunFormatted(run)) continue;

            var result = _grammar.TokenizeLine(line, ruleStack, TimeSpan.MaxValue);
            ruleStack = result.RuleStack;

            if (result.Tokens.Length == 1)
            {
                StyleRun(run, result.Tokens[0].Scopes);
            }
            else
            {
                // Create and style a Run for each token.
                Span span;
                inlines[i] = span = new Span();
                foreach (var token in result.Tokens)
                {
                    var text = line.Substring(token.StartIndex, Math.Min(token.EndIndex - token.StartIndex, line.Length - token.StartIndex));
                    run = new Run(text);
                    StyleRun(run, token.Scopes);
                    span.Inlines.Add(run);
                }
            }
        }
    }

    /// <summary>
    /// Applies styling to a Run based on the token's scopes and the current theme.
    /// </summary>
    /// <param name="run">The Run to style.</param>
    /// <param name="scopes">The scopes associated with the token.</param>
    private static void StyleRun(Run run, IList<string> scopes)
    {
        if (!IsRunFormatted(run)) run.Classes.Add(FormattedClassName);

        var context = RegistryContext.DarkPlus;
        var themeRules = context.Theme.Match(scopes);

        var foregroundId = -1;
        var backgroundId = -1;
        var fontStyle = TextMateSharp.Themes.FontStyle.NotSet;

        // Determine the style from the matched theme rules.
        foreach (var themeRule in themeRules)
        {
            if (foregroundId == -1 && themeRule.foreground > 0)
                foregroundId = themeRule.foreground;

            if (backgroundId == -1 && themeRule.background > 0)
                backgroundId = themeRule.background;

            if (fontStyle == TextMateSharp.Themes.FontStyle.NotSet && themeRule.fontStyle > 0)
                fontStyle = themeRule.fontStyle;
        }

        // Apply foreground color.
        if (foregroundId != -1)
        {
            var colorStr = context.Theme.GetColor(foregroundId);
            if (Color.TryParse(colorStr, out var color))
            {
                run.Foreground = context.GetBrush(color);
            }
        }

        // Apply background color.
        if (backgroundId != -1)
        {
            var colorStr = context.Theme.GetColor(backgroundId);
            if (Color.TryParse(colorStr, out var color))
            {
                run.Background = context.GetBrush(color);
            }
        }

        // Apply font styles.
        if (fontStyle == TextMateSharp.Themes.FontStyle.NotSet) return;

        if ((fontStyle & TextMateSharp.Themes.FontStyle.Italic) != 0) run.FontStyle = FontStyle.Italic;
        if ((fontStyle & TextMateSharp.Themes.FontStyle.Bold) != 0) run.FontWeight = FontWeight.Bold;
        if ((fontStyle & TextMateSharp.Themes.FontStyle.Underline) != 0) ApplyDecoration(TextDecorations.Underline);
        if ((fontStyle & TextMateSharp.Themes.FontStyle.Strikethrough) != 0) ApplyDecoration(TextDecorations.Strikethrough);

        void ApplyDecoration(TextDecorationCollection decorations)
        {
            if (run.TextDecorations is null)
            {
                run.TextDecorations = decorations;
            }
            else
            {
                run.TextDecorations.AddRange(decorations);
            }
        }
    }
}
