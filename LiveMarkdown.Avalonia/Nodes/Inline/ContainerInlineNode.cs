using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;

namespace LiveMarkdown.Avalonia;

/// <summary>
/// A node that represents a container inline (e.g., Span).
/// </summary>
public class ContainerInlineNode<TContainerInline>() : InlinesNode<TContainerInline>(new Span()) where TContainerInline : ContainerInline;