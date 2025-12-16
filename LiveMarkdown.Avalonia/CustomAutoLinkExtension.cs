using Markdig;
using Markdig.Extensions.AutoLinks;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LiveMarkdown.Avalonia;

public class CustomAutoLinkExtension() : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<CustomAutoLinkParser>())
        {
            // Insert the parser before any other parsers
            pipeline.InlineParsers.Insert(0, new CustomAutoLinkParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }
}

internal ref struct CustomValueStringBuilder
{
    public const int StackallocThreshold = 256;

    private char[]? _arrayToReturnToPool;

    private Span<char> _chars;

    private int _pos;

    public int Length
    {
        get
        {
            return _pos;
        }
        set
        {
            _pos = value;
        }
    }

    public ref char this[int index] => ref _chars[index];

    public CustomValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    public override string ToString()
    {
        string result = _chars.Slice(0, _pos).ToString();
        Dispose();
        return result;
    }

    public ReadOnlySpan<char> AsSpan()
    {
        return _chars.Slice(0, _pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        int pos = _pos;
        Span<char> chars = _chars;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    public void Append(char c, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        Span<char> span = _chars.Slice(_pos, count);
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = c;
        }

        _pos += count;
    }

    public void Append(uint i)
    {
        if (i < 10)
        {
            Append((char)(48 + i));
        }
        else
        {
            Append(i.ToString());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string s)
    {
        int pos = _pos;
        if (pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s.CopyTo(_chars.Slice(pos));
        _pos += s.Length;
    }

    public void Append(ReadOnlySpan<char> value)
    {
        if (_pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        int pos = _pos;
        if (pos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = pos + length;
        return _chars.Slice(pos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    //
    // Summary:
    //     Resize the internal buffer either by doubling current buffer size or by adding
    //     additionalCapacityBeyondPos to Markdig.Helpers.ValueStringBuilder._pos whichever
    //     is greater.
    //
    // Parameters:
    //   additionalCapacityBeyondPos:
    //     Number of chars requested beyond current position.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        char[] array = ArrayPool<char>.Shared.Rent((int)Math.Max((uint)(_pos + additionalCapacityBeyondPos), (uint)(_chars.Length * 2)));
        _chars.Slice(0, _pos).CopyTo(array);
        char[] arrayToReturnToPool = _arrayToReturnToPool;
        _chars = (_arrayToReturnToPool = array);
        if (arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(arrayToReturnToPool);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[] arrayToReturnToPool = _arrayToReturnToPool;
        this = default(CustomValueStringBuilder);
        if (arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(arrayToReturnToPool);
        }
    }
}

public class CustomAutoLinkParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoLinkParser"/> class.
    /// </summary>
    public CustomAutoLinkParser()
    {
        OpeningCharacters =
        [
            'h', // for http:// and https://
            //'f', // for ftp://
            //'m', // for mailto:
            //'t', // for tel:
            //'w', // for www.
        ];

        _validPreviousCharacters = SearchValues.Create("*_~(");
    }


    private readonly SearchValues<char> _validPreviousCharacters;

    // This is a particularly expensive parser as it gets called for many common letters.
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Previous char must be a whitespace or a punctuation
        var previousChar = slice.PeekCharExtra(-1);
        if (!previousChar.IsWhiteSpaceOrZero() && !_validPreviousCharacters.Contains(previousChar))
        {
            return false;
        }

        ReadOnlySpan<char> span = slice.AsSpan();

        Debug.Assert(span[0] is 'h' or 'f' or 'm' or 't' or 'w');

        // Precheck URL
        bool mayBeValid = span.Length >= 4 && span[0] switch
        {
            'h' => span.StartsWith("https://", StringComparison.Ordinal) || span.StartsWith("http://", StringComparison.Ordinal),
            //'w' => span.StartsWith("www.", StringComparison.Ordinal), // We won't match http:/www. or /www.xxx
            //'f' => span.StartsWith("ftp://", StringComparison.Ordinal),
            //'m' => span.StartsWith("mailto:", StringComparison.Ordinal),
            //_ => span.StartsWith("tel:", StringComparison.Ordinal),
            _ => false
        };

        return mayBeValid && MatchCore(processor, ref slice);
    }

    private bool MatchCore(InlineProcessor processor, ref StringSlice slice)
    {
        char c = slice.CurrentChar;
        var startPosition = slice.Start;

        // We don't bother disposing the builder as it'll realistically never grow beyond the initial stack size.
        var pendingEmphasis = new CustomValueStringBuilder(stackalloc char[32]);

        // Check that an autolink is possible in the current context
        if (!IsAutoLinkValidInCurrentContext(processor, ref pendingEmphasis))
        {
            return false;
        }

        // Parse URL
        if (!LinkHelper.TryParseUrl(ref slice, out string? link, out _, true))
        {
            return false;
        }

        // If we have any pending emphasis, remove any pending emphasis characters from the end of the link
        if (pendingEmphasis.Length > 0)
        {
            for (int i = link.Length - 1; i >= 0; i--)
            {
                if (pendingEmphasis.AsSpan().Contains(link[i]))
                {
                    slice.Start--;
                }
                else
                {
                    if (i < link.Length - 1)
                    {
                        link = link.Substring(0, i + 1);
                    }
                    break;
                }
            }
        }

        int domainOffset = 0;

        // Post-check URL
        switch (c)
        {
            case 'h':
                if (string.Equals(link, "http://", StringComparison.Ordinal) ||
                    string.Equals(link, "https://", StringComparison.Ordinal))
                {
                    return false;
                }
                domainOffset = link[4] == 's' ? 8 : 7; // https:// or http://
                break;

            case 'w':
                domainOffset = 4; // www.
                break;

            case 'f':
                if (string.Equals(link, "ftp://", StringComparison.Ordinal))
                {
                    return false;
                }
                domainOffset = 6; // ftp://
                break;

            case 't':
                if (string.Equals(link, "tel", StringComparison.Ordinal))
                {
                    return false;
                }
                break;

            case 'm':
                int atIndex = link.IndexOf('@');
                if (atIndex == -1 ||
                    atIndex == 7) // mailto:@ - no email part
                {
                    return false;
                }
                domainOffset = atIndex + 1;
                break;
        }

        // Do not need to check if a telephone number is a valid domain
        if (c != 't' && !LinkHelper.IsValidDomain(link, domainOffset, allowDomainWithoutPeriod: true))
        {
            return false;
        }

        var inline = new LinkInline()
        {
            Span =
            {
                Start = processor.GetSourcePosition(startPosition, out int line, out int column),
            },
            Line = line,
            Column = column,
            Url = link,
            IsClosed = true,
            IsAutoLink = true,
        };

        int skipFromBeginning = c switch
        {
            'm' => 7, // For mailto: skip "mailto:" for content
            't' => 4, // Same but for tel:
            _ => 0
        };

        inline.Span.End = inline.Span.Start + link.Length - 1;
        inline.UrlSpan = inline.Span;
        inline.AppendChild(new LiteralInline()
        {
            Span = inline.Span,
            Line = line,
            Column = column,
            Content = new StringSlice(slice.Text, startPosition + skipFromBeginning, startPosition + link.Length - 1),
            IsClosed = true
        });
        processor.Inline = inline;

        return true;
    }

    private static bool IsAutoLinkValidInCurrentContext(InlineProcessor processor, ref CustomValueStringBuilder pendingEmphasis)
    {
        // Case where there is a pending HtmlInline <a>
        var currentInline = processor.Inline;
        while (currentInline != null)
        {
            if (currentInline is HtmlInline htmlInline)
            {
                // If we have a </a> we don't expect nested <a>
                if (htmlInline.Tag.StartsWith("</a", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // If there is a pending <a>, we can't allow a link
                if (htmlInline.Tag.StartsWith("<a", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            // Check previous sibling and parents in the tree
            currentInline = currentInline.PreviousSibling ?? currentInline.Parent;
        }

        // Check that we don't have any pending brackets opened (where we could have a possible markdown link)
        // NOTE: This assume that [ and ] are used for links, otherwise autolink will not work properly
        currentInline = processor.Inline;
        int countBrackets = 0;
        while (currentInline != null)
        {
            if (currentInline is LinkDelimiterInline linkDelimiterInline && linkDelimiterInline.IsActive)
            {
                if (linkDelimiterInline.Type == DelimiterType.Open)
                {
                    countBrackets++;
                }
                else if (linkDelimiterInline.Type == DelimiterType.Close)
                {
                    countBrackets--;
                }
            }
            else
            {
                // Record all pending characters for emphasis
                if (currentInline is EmphasisDelimiterInline emphasisDelimiter)
                {
                    if (!pendingEmphasis.AsSpan().Contains(emphasisDelimiter.DelimiterChar))
                    {
                        pendingEmphasis.Append(emphasisDelimiter.DelimiterChar);
                    }
                }
            }
            currentInline = currentInline.Parent;
        }

        return countBrackets <= 0;
    }
}