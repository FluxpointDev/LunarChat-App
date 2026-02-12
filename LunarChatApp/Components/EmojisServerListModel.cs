using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Servers;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.Components;

public partial class EmojisServerListModel : ViewModelBase
{
    private readonly ServiceManager services;
    public EmojisServerListModel(ServiceManager sv, RestServer server)
    {
        services = sv;
        name = server.Name;
        serverId = server.Id;
        emojisList = new ObservableCollection<EmojiListItem>();
        if (services.Client.WebSocket.State.Servers.TryGetValue(server.Id, out var serverState))
        {
            var list = serverState.Emojis.Values.Select(x => new EmojiListItem
            {
                DataContext = new EmojiListItemModel(services, x)
            });
            emojisList.Add(list);
        }
    }

    [ObservableProperty]
    public string name;

    public ulong serverId;

    [ObservableProperty]
    private ObservableCollection<EmojiListItem> emojisList;
}
