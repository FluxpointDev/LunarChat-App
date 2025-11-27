using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Markdig.Extensions.TaskLists;

namespace LiveMarkdown.Avalonia;

public class TaskListNode : InlineNode<TaskList>
{
    public override Inline Inline { get; }

    private readonly CheckBox checkBox;

    public TaskListNode()
    {
        Inline = new InlineUIContainer
        {
            Classes = { "TaskList" },
            Child = checkBox = new CheckBox
            {
                Classes = { "TaskList" }
            }
        };
    }

    protected override bool UpdateCore(
        DocumentNode documentNode,
        TaskList taskList,
        in ObservableStringBuilderChangedEventArgs change,
        CancellationToken cancellationToken)
    {
        checkBox.IsChecked = taskList.Checked;
        return true;
    }
}