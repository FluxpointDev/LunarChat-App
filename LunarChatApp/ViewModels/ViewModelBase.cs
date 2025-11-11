using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace LunarChatApp.ViewModels;

public abstract class ViewModelBase : ObservableObject, IDisposable
{
    private bool _disposed;

    public virtual void Dispose()
    {
        if (_disposed) return;

        // Clear errors and event handlers
        //ClearAllErrors();
        //ErrorsChanged = null;

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~ViewModelBase()
    {
        Dispose();
    }
}
