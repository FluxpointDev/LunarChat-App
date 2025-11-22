using Avalonia.Controls;
using LunarChatApp.ViewModels;
using System;
using System.Collections.Generic;

namespace LunarChatApp.Services;

public class DialogService
{
    public Action<DialogMenu> OnDialogOpen;
    public Action OnDialogClose;

    internal readonly Dictionary<Type, Type> CustomDialogs = [];

    public DialogService Register<TView, TContext>() where TView : UserControl where TContext : ViewModelBase
    {
        CustomDialogs.Add(typeof(TContext), typeof(TView));
        return this;
    }

    public DialogMenu Create<Model>(Model model, string title)
    {
        if (!CustomDialogs.TryGetValue(model.GetType(), out Type? modelType))
            throw new InvalidOperationException($"Custom dialog with {nameof(model)} is not registered.");

        UserControl? control = Activator.CreateInstance(modelType) as UserControl;
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
}