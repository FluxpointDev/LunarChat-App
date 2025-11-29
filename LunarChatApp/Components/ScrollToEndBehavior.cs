using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using LunarChatApp.Services;
using LunarChatApp.ViewModels;
using System;
using System.Diagnostics;

namespace LunarChatApp.Components;

public class ScrollToEndBehavior : Behavior<ScrollViewer>
{
    private bool _shouldScrollToEnd = true;
    private bool _scrolling;
    private ServiceManager services;
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is { })
        {
            AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
            AssociatedObject.TemplateApplied += AssociatedObjectOnTemplateApplied;

        }
    }


    private void AssociatedObjectOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        var sw = sender as ScrollViewer;
        sw.ScrollChanged += SwOnScrollChanged;
        services = (sw.DataContext as ChannelModel).services;
        services.State.Socket.WebSocket.OnMessageRecieved += WebSocket_OnMessageRecieved;
    }

    private void WebSocket_OnMessageRecieved(Shared.WebSocket.Events.MessageRecievedEvent message)
    {
        if (_shouldScrollToEnd || message.user.Id == services.State.Socket.CurrentId)
        {
            Debug.WriteLine("New Message Scroll: " + _scrolling);
            if (AssociatedObject != null)
                AssociatedObject.ScrollToEnd();
        }
    }

    private void Children_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Debug.WriteLine("Trigger Scroll Collection: " + (AssociatedObject.DataContext as ChannelModel).MessagesFinished);
        if (Math.Abs(AssociatedObject.Offset.Y - AssociatedObject.Extent.Height + AssociatedObject.Viewport.Height) < 5)
        {
            Debug.WriteLine("Now scroll");
            AssociatedObject.ScrollToEnd();
        }
    }

    bool FirstScroll = false;

    private void SwOnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sw)
        {
            _shouldScrollToEnd = Math.Abs(sw.Offset.Y - sw.Extent.Height + sw.Viewport.Height) < 5; // need to define some px of tolerance here
            Debug.WriteLine("Should Scroll:" + _shouldScrollToEnd);
            //if (!_shouldScrollToEnd && FirstScoll)
            //{
            //    AssociatedObject.ScrollToEnd();
            //}
        }
    }

    private void AssociatedObjectOnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        Debug.WriteLine("Size changed: " + _shouldScrollToEnd);
        if (_shouldScrollToEnd)
        {
            _scrolling = true;
            AssociatedObject.ScrollToEnd();
            _scrolling = false;
        }
    }
}
