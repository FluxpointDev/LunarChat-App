// @author https://github.com/DearVa

using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media.TextFormatting.Unicode;
using Markdig.Syntax;

namespace LiveMarkdown.Avalonia;

public partial class MarkdownRenderer
{
    /// <summary>
    /// Utility class to map <see cref="Control"/> to <see cref="BlockNode{TInline}"/> itself
    /// </summary>
    /// <param name="target"></param>
    public class InlinesProxy(InlineCollection target)
    {
        public int Count => children.Count;

        public InlineNode this[int index]
        {
            get => children[index];
            set
            {
                target[index] = value.Inline;
                children[index] = value;
            }
        }

        private readonly List<InlineNode> children = [];

        public void Add(InlineNode node)
        {
            children.Add(node);
            target.Add(node.Inline);
            Debug.Assert(children.Count == target.Count, "Children count mismatch");
        }

        public void RemoveAt(int index)
        {
            var node = children[index];
            target.Remove(node.Inline);
            children.RemoveAt(index);
            Debug.Assert(children.Count == target.Count, "Children count mismatch");
        }
    }

    /// <summary>
    /// Utility class to map <see cref="Control"/> to <see cref="BlockNode{TBlock}"/> itself
    /// </summary>
    /// <param name="target"></param>
    public class BlocksProxy(Controls target)
    {
        public class MockBlockNode(Control control) : BlockNode
        {
            private const string NotSupportedMessage = "This node is a mock and does not support this operation.";

            public override Control Control => control;

            private Control control = control;

            public void SetControl(Control control) => this.control = control;

            protected override bool UpdateCore(
                DocumentNode documentNode,
                MarkdownObject markdownObject,
                in ObservableStringBuilderChangedEventArgs change,
                CancellationToken cancellationToken)
            {
                throw new NotSupportedException(NotSupportedMessage);
            }
        }

        public int Count => children.Count;

        public BlockNode this[int index]
        {
            get => children[index];
            set
            {
                target[index] = value.Control;
                children[index] = value;
            }
        }

        private readonly List<BlockNode> children = [];

        public void Add(BlockNode node)
        {
            children.Add(node);
            target.Add(node.Control);
            Debug.Assert(children.Count == target.Count, "Children count mismatch");
        }

        /// <summary>
        /// Adds a control to the target collection for special cases where a mock node is needed.
        /// This should be treated carefully as it adds a mock node.
        /// </summary>
        /// <param name="control"></param>
        public void Add(Control control)
        {
            children.Add(new MockBlockNode(control));
            target.Add(control);
        }

        public void SetControlAt(int index, Control control)
        {
            target[index] = control;
            if (children[index] is MockBlockNode mockBlockNode) mockBlockNode.SetControl(control);
            else children[index] = new MockBlockNode(control);
        }

        public void RemoveAt(int index)
        {
            var node = children[index];
            target.Remove(node.Control);
            children.RemoveAt(index);
            Debug.Assert(children.Count == target.Count, "Children count mismatch");
        }

        public void Clear()
        {
            target.Clear();
            children.Clear();
        }
    }

    private static class StringUtils
    {
        private enum CharClass
        {
            CharClassUnknown,
            CharClassWhitespace,
            CharClassAlphaNumeric,
        }

        public static bool IsEol(char c)
        {
            return c is '\r' or '\n';
        }

        public static bool IsStartOfWord(string text, int index)
        {
            if (index >= text.Length)
            {
                return false;
            }

            var codepoint = new Codepoint(text[index]);

            // A 'word' starts with an AlphaNumeric or some punctuation symbols immediately
            // preceeded by lwsp.
            if (index > 0)
            {
                var previousCodepoint = new Codepoint(text[index - 1]);

                if (!previousCodepoint.IsWhiteSpace)
                {
                    return false;
                }

                if (previousCodepoint.IsBreakChar)
                {
                    return true;
                }
            }

            switch (codepoint.GeneralCategory)
            {
                case GeneralCategory.LowercaseLetter:
                case GeneralCategory.TitlecaseLetter:
                case GeneralCategory.UppercaseLetter:
                case GeneralCategory.DecimalNumber:
                case GeneralCategory.LetterNumber:
                case GeneralCategory.OtherNumber:
                case GeneralCategory.DashPunctuation:
                case GeneralCategory.InitialPunctuation:
                case GeneralCategory.OpenPunctuation:
                case GeneralCategory.CurrencySymbol:
                case GeneralCategory.MathSymbol:
                    return true;

                // TODO: How do you do this in .NET?
                // case UnicodeCategory.OtherPunctuation:
                //    // words cannot start with '.', but they can start with '&' or '*' (for example)
                //    return g_unichar_break_type(buffer->text[index]) == G_UNICODE_BREAK_ALPHABETIC;
                default:
                    return false;
            }
        }

        public static bool IsEndOfWord(string text, int index)
        {
            if (index >= text.Length)
            {
                return true;
            }

            var codepoint = new Codepoint(text[index]);

            if (!codepoint.IsWhiteSpace)
            {
                return false;
            }
            // A 'word' starts with an AlphaNumeric or some punctuation symbols immediately
            // preceeded by lwsp.
            if (index > 0)
            {
                if (index + 1 < text.Length)
                {
                    var nextCodePoint = new Codepoint(text[index + 1]);

                    if (nextCodePoint.IsBreakChar)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }

            }

            switch (codepoint.GeneralCategory)
            {
                case GeneralCategory.LowercaseLetter:
                case GeneralCategory.TitlecaseLetter:
                case GeneralCategory.UppercaseLetter:
                case GeneralCategory.DecimalNumber:
                case GeneralCategory.LetterNumber:
                case GeneralCategory.OtherNumber:
                case GeneralCategory.DashPunctuation:
                case GeneralCategory.InitialPunctuation:
                case GeneralCategory.OpenPunctuation:
                case GeneralCategory.CurrencySymbol:
                case GeneralCategory.MathSymbol:
                    return false;

                // TODO: How do you do this in .NET?
                // case UnicodeCategory.OtherPunctuation:
                //    // words cannot start with '.', but they can start with '&' or '*' (for example)
                //    return g_unichar_break_type(buffer->text[index]) == G_UNICODE_BREAK_ALPHABETIC;
                default:
                    return true;
            }
        }

        public static int PreviousWord(string text, int cursor)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            cursor = Math.Min(cursor, text.Length);

            int begin;
            int i;
            int cr;
            int lf;

            lf = LineBegin(text, cursor) - 1;

            if (lf > 0 && text[lf] == '\n' && text[lf - 1] == '\r')
            {
                cr = lf - 1;
            }
            else
            {
                cr = lf;
            }

            // if the cursor is at the beginning of the line, return the end of the prev line
            if (cursor - 1 == lf)
            {
                return (cr > 0) ? cr : 0;
            }

            CharClass cc = GetCharClass(text[cursor - 1]);
            begin = lf + 1;
            i = cursor;

            // skip over the word, punctuation, or run of whitespace
            while (i > begin && GetCharClass(text[i - 1]) == cc)
            {
                i--;
            }

            // if the cursor was at whitespace, skip back a word too
            if (cc == CharClass.CharClassWhitespace && i > begin)
            {
                cc = GetCharClass(text[i - 1]);
                while (i > begin && GetCharClass(text[i - 1]) == cc)
                {
                    i--;
                }
            }

            return i;
        }

        public static int NextWord(string text, int cursor)
        {
            int i, lf, cr;

            cr = LineEnd(text, cursor);

            if (cursor >= text.Length)
            {
                return cursor;
            }

            if (cr < text.Length && text[cr] == '\r' && cr + 1 < text.Length && text[cr + 1] == '\n')
            {
                lf = cr + 1;
            }
            else
            {
                lf = cr;
            }

            // if the cursor is at the end of the line, return the starting offset of the next line
            if (cursor == cr || cursor == lf)
            {
                if (lf < text.Length)
                {
                    return lf + 1;
                }

                return cursor;
            }

            i = cursor;

            // skip any whitespace after the word/punct
            while (i < cr && char.IsWhiteSpace(text[i]))
            {
                i++;
            }

            if (i >= cr)
            {
                return i;
            }

            var cc = GetCharClass(text[i]);

            // skip over the word, punctuation, or run of whitespace
            while (i < cr && GetCharClass(text[i]) == cc)
            {
                i++;
            }

            return i;
        }

        private static CharClass GetCharClass(char c)
        {
            if (char.IsWhiteSpace(c))
            {
                return CharClass.CharClassWhitespace;
            }
            else if (char.IsLetterOrDigit(c))
            {
                return CharClass.CharClassAlphaNumeric;
            }
            else
            {
                return CharClass.CharClassUnknown;
            }
        }

        private static int LineBegin(string text, int pos)
        {
            while (pos > 0 && !IsEol(text[pos - 1]))
            {
                pos--;
            }

            return pos;
        }

        private static int LineEnd(string text, int cursor, bool include = false)
        {
            while (cursor < text.Length && !IsEol(text[cursor]))
            {
                cursor++;
            }

            if (include && cursor < text.Length)
            {
                if (text[cursor] == '\r' && text[cursor + 1] == '\n')
                {
                    cursor += 2;
                }
                else
                {
                    cursor++;
                }
            }

            return cursor;
        }
    }
}

internal static class ClassesExtension
{
    /// <summary>
    /// Ensures that the specified class name is present in the given <see cref="Classes"/> collection.
    /// </summary>
    /// <param name="classes"></param>
    /// <param name="name"></param>
    public static void EnsureClassName(this Classes classes, string name)
    {
        if (!classes.Contains(name))
        {
            classes.Add(name);
        }
    }

    /// <summary>
    /// Ensures that the specified class name with prefix and suffix is present in the given <see cref="Classes"/> collection.
    /// </summary>
    /// <param name="classes"></param>
    /// <param name="prefix"></param>
    /// <param name="suffix"></param>
    public static void EnsureClassName<TSuffix>(this Classes classes, string prefix, TSuffix suffix)
    {
        var index = -1;
        for (var i = 0; i < classes.Count; i++)
        {
            var cls = classes[i];
            if (cls.StartsWith(prefix, StringComparison.Ordinal))
            {
                index = i;
                break;
            }
        }

        var name = prefix + suffix;
        if (index == -1)
        {
            classes.Add(name);
        }
        else if (classes[index] != name)
        {
            // classes[index] = name; // This may not work in Avalonia
            classes.RemoveAt(index);
            classes.Add(name);
        }
    }
}