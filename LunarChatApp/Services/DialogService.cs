using Avalonia.Controls;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Services;

public class DialogService
{
    public Action<DialogMenu> OnDialogOpen;
    public Action OnDialogClose;


    public DialogMenu Create<Model>(UserControl control, Model model, string title)
    {
        if (control == null)
            throw new InvalidOperationException("Dialog control is not set.");

        control.DataContext = model;
        DialogMenu menu = new DialogMenu
        {
            service = this,
            Title = title,
            Control = control,
        };
        return menu;
    }
}
public class DialogMenu
{
    internal DialogService service;
    public Action<UserControl> OnSubmit;
    public Func<UserControl, Task> OnSubmitAsync;
    public string? Title;
    public UserControl? Control;

    public void Open()
    {
        service.OnDialogOpen.Invoke(this);
    }

    public DialogMenu WithSubmit(Action<UserControl> action)
    {
        OnSubmit = action;
        return this;
    }

    public DialogMenu WithSubmit(Func<UserControl, Task> action)
    {
        OnSubmitAsync = action;
        return this;
    }
}