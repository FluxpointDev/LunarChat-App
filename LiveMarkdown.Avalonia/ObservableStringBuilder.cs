using System.Text;

namespace LiveMarkdown.Avalonia;

public readonly record struct ObservableStringBuilderChangedEventArgs(string NewString, int StartIndex, int Length);

public delegate void ObservableStringBuilderChangedEventHandler(in ObservableStringBuilderChangedEventArgs e);

/// <summary>
/// A string builder that raises events when its content changes.
/// </summary>
public class ObservableStringBuilder
{
    /// <summary>
    /// Gets the current length of the string builder.
    /// </summary>
    public int Length => stringBuilder.Length;

    /// <summary>
    /// Raised when the content of the string builder changes.
    /// </summary>
    public event ObservableStringBuilderChangedEventHandler? Changed;

    private readonly StringBuilder stringBuilder = new();

    /// <summary>
    /// Appends a string to the string builder and raises an event.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ObservableStringBuilder Append(string? value)
    {
        if (string.IsNullOrEmpty(value)) return this;
        stringBuilder.Append(value);
        Changed?.Invoke(
            new ObservableStringBuilderChangedEventArgs(
                ToString(),
                // ReSharper disable once RedundantSuppressNullableWarningExpression
                stringBuilder.Length - value!.Length,
                value.Length));
        return this;
    }

    /// <summary>
    /// Appends a string followed by a newline to the string builder and raises an event.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ObservableStringBuilder AppendLine(string? value = null)
    {
        if (string.IsNullOrEmpty(value)) return this;
        stringBuilder.AppendLine(value);
        Changed?.Invoke(
            new ObservableStringBuilderChangedEventArgs(
                ToString(),
                // ReSharper disable once RedundantSuppressNullableWarningExpression
                stringBuilder.Length - value!.Length - Environment.NewLine.Length,
                value.Length + Environment.NewLine.Length));
        return this;
    }

    /// <summary>
    /// Clears the string builder and raises an event with the previous content.
    /// </summary>
    /// <returns></returns>
    public ObservableStringBuilder Clear()
    {
        var length = stringBuilder.Length;
        stringBuilder.Clear();
        Changed?.Invoke(
            new ObservableStringBuilderChangedEventArgs(
                string.Empty,
                0,
                length));
        return this;
    }

    public override string ToString()
    {
        return stringBuilder.ToString();
    }
}