using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatSharp.Core.Channels;
using LunarChatSharp.Rest.Channels;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace LunarChatApp.Views.Channels;

public partial class CategoryItemModel : ViewModelBase
{
    private readonly ServiceManager services;
    public CategoryItemModel(ServiceManager sv, RestChannel channel, bool canManage)
    {
        services = sv;
        isRootCategory = string.IsNullOrEmpty(channel.Id);
        id = channel.Id;
        name = channel.Name;
        _channelsList = new ObservableCollection<ChannelItem>(services.State.CurrentServer.Channels.Values.Where(x => x.Type != ChannelType.Category && (isRootCategory ? string.IsNullOrEmpty(x.ParentId) : x.ParentId == channel.Id)).OrderBy(x => x.Position).Select(x => new ChannelItem() { DataContext = new ChannelItemModel(services, services.State, x, canManage) }));
    }

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private bool isRootCategory;

    public string id;

    [RelayCommand]
    public void ToggleCategory()
    {
        toggled = !toggled;
        ArrowIcon = toggled ? MaterialIconKind.KeyboardArrowDown : MaterialIconKind.KeyboardArrowRight;
        Debug.WriteLine("Toggle: " + toggled.ToString());
        foreach (var i in ChannelsList)
        {
            ChannelItemModel? model = i.DataContext as ChannelItemModel;
            if (model == null)
                continue;

            if (!toggled && services.State.CurrentChannel?.Id == model.id)
                continue;

            Debug.WriteLine("Toggle: " + model.Name);

            model.Toggled = toggled;
        }
    }

    private bool toggled = true;

    [ObservableProperty]
    private MaterialIconKind arrowIcon = MaterialIconKind.KeyboardArrowDown;

    [RelayCommand]
    public void CreateChannel()
    {

    }

    [ObservableProperty]
    private ObservableCollection<ChannelItem> _channelsList;
}
