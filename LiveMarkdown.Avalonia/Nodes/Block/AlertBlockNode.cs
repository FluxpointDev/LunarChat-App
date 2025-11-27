using Markdig.Extensions.Alerts;

namespace LiveMarkdown.Avalonia;

public sealed class AlertBlockNode : ContainerBlockNode<AlertBlock>
{
    public AlertBlockNode()
    {
        container.Classes.Add("AlertBlock");
    }
}