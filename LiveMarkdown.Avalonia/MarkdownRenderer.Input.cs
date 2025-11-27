#pragma warning disable CS0618 // MathUtilities is Obsolete

using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Utilities;

namespace LiveMarkdown.Avalonia;

public partial class MarkdownRenderer
{
    public string SelectedText
    {
        get
        {
            if (selectionBlocks.Count <= 0) return string.Empty;

            var copyStringBuilder = new StringBuilder();
            Rect? previousBounds = null;
            foreach (var block in selectionBlocks.Where(b => b.FindLogicalAncestorOfType<MarkdownTextBlock>() is null))
            {
                var bounds = TranslateBoundsToGlobal(block);
                if (previousBounds is not null && bounds.Y >= previousBounds.Value.Bottom) copyStringBuilder.AppendLine();
                copyStringBuilder.Append(block.SelectedText);
                previousBounds = bounds;
            }

            return copyStringBuilder.ToString();
        }
    }

    private static MarkdownTextBlock? GetMarkdownTextBlock(PointerEventArgs e)
    {
        var element = e.Source as StyledElement;
        while (element != null)
        {
            switch (element)
            {
                case MarkdownTextBlock stb:
                    return stb;
                case MarkdownRenderer:
                    return null;
                default:
                    element = element.Parent;
                    break;
            }
        }
        return null;
    }

    private static bool IsClickInsideMarkdownTextBlock(PointerEventArgs e)
    {
        var element = e.Source as StyledElement;
        while (element != null)
        {
            switch (element)
            {
                case Button:
                case Track:
                    return false;
                case MarkdownTextBlock:
                case MarkdownRenderer:
                    return true;
                default:
                    element = element.Parent;
                    break;
            }
        }
        return false;
    }

    private MarkdownTextBlock? selectionStartBlock;
    private Rect startBlockGlobalBounds;
    private int startBlockSelectionStart;
    private readonly List<MarkdownTextBlock> selectionBlocks = [];

    /// <summary>
    /// Translates the bounds of a visual element to global(this) coordinates.
    /// </summary>
    /// <param name="visual"></param>
    /// <returns></returns>
    private Rect TranslateBoundsToGlobal(Visual visual)
    {
        var topLeft = visual.TranslatePoint(new Point(), this) ?? new Point();
        var bounds = visual.Bounds;
        var bottomRight = visual.TranslatePoint(new Point(bounds.Width, bounds.Height), this) ?? new Point();
        return new Rect(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }

    private MarkdownTextBlock? FindMarkdownTextBlockAtPoint(PointerEventArgs e, Point point)
    {
        var result = GetMarkdownTextBlock(e);
        if (result is not null) return result;

        MarkdownTextBlock? previous = null;
        foreach (var current in documentNode.TextBlocks.OrderBy(b => b.SourceSpan.Start))
        {
            var bounds = TranslateBoundsToGlobal(current);

            // 1. check if `point` is inside the bounds of the current MarkdownTextBlock
            if (bounds.Contains(point)) return current;

            // 2. check if `point` is between the bounds of the previous and current MarkdownTextBlock
            if (previous is not null &&
                bounds.Y > point.Y || bounds.Bottom >= point.Y && bounds.X > point.X)
            {
                return previous;
            }

            previous = current;
        }

        return null;
    }

    private void HandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Focus();

        if (!IsClickInsideMarkdownTextBlock(e))
        {
            // if the click is not inside a MarkdownTextBlock, we do not handle the event
            return;
        }

        var clickInfo = e.GetCurrentPoint(this);
        selectionStartBlock = FindMarkdownTextBlockAtPoint(e, clickInfo.Position);
        if (selectionStartBlock is null)
        {
            // if no MarkdownTextBlock was found, we do not handle the event
            return;
        }

        if (clickInfo.Properties.IsLeftButtonPressed)
        {
            foreach (var selectionBlock in selectionBlocks)
            {
                // We do not clear selection on the selection start block.
                // Or the selection start block will lose its selection and cause remeasure
                // which will make the selection jumpy and render incorrectly.
                if (!ReferenceEquals(selectionBlock, selectionStartBlock))
                {
                    selectionBlock.ClearSelection();
                }
            }

            selectionBlocks.Clear();
        }

        startBlockGlobalBounds = TranslateBoundsToGlobal(selectionStartBlock);
        selectionBlocks.Add(selectionStartBlock);

        var text = selectionStartBlock.ActualText;
        if (clickInfo.Properties.IsLeftButtonPressed)
        {
            var padding = selectionStartBlock.Padding;
            var point = e.GetPosition(selectionStartBlock) - new Point(padding.Left, padding.Top);
            var textPosition = selectionStartBlock.TextLayoutHitTestPoint(point);
            var wordSelectionStart = MathUtilities.Clamp(startBlockSelectionStart, 0, text.Length);
            var clickToSelect = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
            switch (e.ClickCount)
            {
                case 1:
                {
                    if (clickToSelect)
                    {
                        var previousWord = StringUtils.PreviousWord(text, textPosition);

                        if (textPosition > wordSelectionStart)
                        {
                            SetCurrentValue(SelectableTextBlock.SelectionEndProperty, StringUtils.NextWord(text, textPosition));
                        }

                        if (textPosition < wordSelectionStart || previousWord == wordSelectionStart)
                        {
                            SetCurrentValue(SelectableTextBlock.SelectionStartProperty, previousWord);
                        }
                    }
                    else
                    {
                        selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, textPosition);
                        selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, textPosition);
                        startBlockSelectionStart = textPosition;
                    }

                    break;
                }
                case 2:
                {
                    if (!StringUtils.IsStartOfWord(text, textPosition))
                    {
                        selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, StringUtils.PreviousWord(text, textPosition));
                    }

                    startBlockSelectionStart = wordSelectionStart;

                    if (!StringUtils.IsEndOfWord(text, textPosition))
                    {
                        selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, StringUtils.NextWord(text, textPosition));
                    }

                    break;
                }
                case 3:
                {
                    // select all
                    selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, 0);
                    selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, text.Length);
                    startBlockSelectionStart = wordSelectionStart;
                    break;
                }
            }
        }

        e.Pointer.Capture(this);
        e.Handled = true;
    }

    private void HandlePointerMoved(object? sender, PointerEventArgs e)
    {
        // selection should not change during pointer move if the user right clicks
        var clickInfo = e.GetCurrentPoint(this);
        if (selectionStartBlock is null || !Equals(e.Pointer.Captured, this) || !clickInfo.Properties.IsLeftButtonPressed) return;

        var selectionEndBlock = FindMarkdownTextBlockAtPoint(e, clickInfo.Position);
        if (selectionEndBlock is null) return;

        var text = selectionEndBlock.ActualText;
        var padding = selectionEndBlock.Padding;

        var point = e.GetPosition(selectionEndBlock) - new Point(padding.Left, padding.Top);

        point = new Point(
            MathUtilities.Clamp(point.X, 0, Math.Max(selectionEndBlock.TextLayout.WidthIncludingTrailingWhitespace, 0)),
            MathUtilities.Clamp(point.Y, 0, Math.Max(selectionEndBlock.TextLayout.Height, 0)));

        var textPosition = selectionEndBlock.TextLayoutHitTestPoint(point);

        if (Equals(selectionEndBlock, selectionStartBlock))
        {
            // We are selecting inside the same `MarkdownTextBlock`
            var selectionStart = Math.Min(startBlockSelectionStart, textPosition);
            var selectionEnd = Math.Max(startBlockSelectionStart, textPosition);
            selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, selectionStart);
            selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, selectionEnd);

            // remove all blocks after the current index
            for (var i = selectionBlocks.Count - 1; i > 0; i--)
            {
                selectionBlocks[i].ClearSelection();
                selectionBlocks.RemoveAt(i);
            }
        }
        else
        {
            // Not in the same `MarkdownTextBlock`, we need to get the range that we want to select

            // 1. determine the direction of selection
            //
            //     reversed      |     reversed      |     reversed
            // ------------------+-------------------+-------------------
            //     reversed      |   pointerDownSTB  |
            // ------------------+-------------------+-------------------
            //                   |                   |
            var reversed =
                clickInfo.Position.Y < startBlockGlobalBounds.Y ||
                clickInfo.Position.X < startBlockGlobalBounds.X &&
                clickInfo.Position.Y <= startBlockGlobalBounds.Bottom;

            var blocks = documentNode.TextBlocks.Where(b => b.FindLogicalAncestorOfType<MarkdownTextBlock>() is null);
            int selectionStart, selectionEnd;
            if (reversed)
            {
                selectionStart = 0;
                selectionEnd = startBlockSelectionStart;
                blocks = blocks.OrderByDescending(b => b.SourceSpan.Start);
            }
            else
            {
                selectionStart = startBlockSelectionStart;
                selectionEnd = selectionStartBlock.ActualText.Length;
                blocks = blocks.OrderBy(b => b.SourceSpan.Start);
            }
            selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, selectionStart);
            selectionStartBlock.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, selectionEnd);

            // 3. Enumerate the blocks from the selection start block to the selection end block
            var index = 0;
            blocks = blocks.SkipWhile(b => !ReferenceEquals(b, selectionStartBlock)).Skip(1);
            foreach (var block in blocks)
            {
                index++; // starting from 1, because we already added the selection start block

                // Updates the `selectionBlocks`
                if (selectionBlocks.Count > index && !ReferenceEquals(selectionBlocks[index], block))
                {
                    // `selectionBlocks` is not empty and the current block is different from the one in the list
                    // we need to remove the old blocks after index
                    for (var i = selectionBlocks.Count - 1; i >= index; i--)
                    {
                        selectionBlocks[i].ClearSelection();
                        selectionBlocks.RemoveAt(i);
                    }
                }

                // After removing the old blocks, we can add the current block if count is less than or equal to index
                // or the current block is not already in the list
                if (selectionBlocks.Count <= index)
                {
                    selectionBlocks.Add(block);
                }

                if (ReferenceEquals(block, selectionEndBlock))
                {
                    if (reversed)
                    {
                        block.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, textPosition);
                        block.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, text.Length);
                    }
                    else
                    {
                        block.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, 0);
                        block.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, textPosition);
                    }

                    break;
                }

                // If we are not at the end block, select all text in the block
                block.SelectAll();
            }

            // remove all blocks after the current index
            for (var i = selectionBlocks.Count - 1; i > index; i--)
            {
                selectionBlocks[i].ClearSelection();
                selectionBlocks.RemoveAt(i);
            }
        }

        e.Handled = true;
    }

    private void HandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!Equals(e.Pointer.Captured, this)) return;

        e.Pointer.Capture(null);
    }

    private void HandleKeyDown(object? sender, KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;

        bool Match(List<KeyGesture> gestures) => gestures.Any(g => g.Matches(e));

        if (Match(keymap.Copy))
        {
            Copy();
            e.Handled = true;
        }
        else if (Match(keymap.SelectAll))
        {
            foreach (var block in this.GetLogicalDescendants().OfType<MarkdownTextBlock>()) block.SelectAll();
            e.Handled = true;
        }
    }

    public async void Copy()
    {
        try
        {
            if (TopLevel.GetTopLevel(this) is not { Clipboard: { } clipboard }) return;

            var text = SelectedText;
            if (string.IsNullOrEmpty(text)) return;

            await clipboard.SetTextAsync(text);
        }
        catch
        {
            // ignore any exceptions during copy operation
        }
    }
}

#pragma warning restore CS0618 // MathUtilities is Obsolete