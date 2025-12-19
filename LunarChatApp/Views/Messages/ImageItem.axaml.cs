using Avalonia.Controls;
using System.Diagnostics;

namespace LunarChatApp;

public partial class ImageItem : UserControl
{
    public ImageItem()
    {
        InitializeComponent();
    }

    private void AsyncImage_Failed(object? sender, Avalonia.Labs.Controls.AsyncImage.AsyncImageFailedEventArgs e)
    {
        Debug.WriteLine("Failed");
    }

    private void AsyncImage_Opened(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Debug.WriteLine("Success");
    }
}