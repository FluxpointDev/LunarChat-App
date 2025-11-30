using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelItemModel : ViewModelBase
{
    private TestState state;
    private RestChannel channel;
    private ServiceManager services;
    public string id;
    public ChannelItemModel(ServiceManager sv, TestState st, RestChannel chan)
    {
        id = chan.Id;
        state = st;
        channel = chan;
        services = sv;
        Name = chan.Name;
        isOwner = st.Socket.CurrentServer?.Server.OwnerId == st.Socket.CurrentId;
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool isOwner;

    [RelayCommand]
    public void SelectChannel()
    {
        services.PageManager.SwitchServerChannel(services, channel);
    }

    [RelayCommand]
    public void OpenChannelSettings()
    {
        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services, channel)
        });
    }
}
