using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Servers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Components;

public partial class EmojisMenuModel : ViewModelBase
{
    private readonly ServiceManager services;
    public EmojisMenuModel(ServiceManager sv)
    {
        services = sv;
        services.Client.OnEmojiCreate += EmojiCreate;
        services.Client.OnEmojiUpdate += EmojiUpdate;
        services.Client.OnEmojiDelete += EmojiDelete;
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            List<EmojisServerList> list = new List<EmojisServerList>();
            foreach (var i in services.Client.WebSocket.State.Servers.Values)
            {
                list.Add(new EmojisServerList { DataContext = new EmojisServerListModel(services, i.Server) });
            }
            ServersList = new ObservableCollection<EmojisServerList>(list);
        });

    }

    [ObservableProperty]
    public bool openMenu;

    private async Task EmojiDelete(RestServer server, RestEmoji emoji)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (var i in ServersList)
            {
                EmojisServerListModel? server = (i.DataContext as EmojisServerListModel);
                if (server == null)
                    continue;
                EmojiListItem? model = server.EmojisList.FirstOrDefault(x => (x.DataContext as EmojiListItemModel)?.emojiId == emoji.Id);
                if (model == null)
                    continue;

                server.EmojisList.Remove(model);
                break;
            }
        });
    }

    private async Task EmojiUpdate(RestServer server, RestEmoji emoji, EmojiUpdateEvent @event)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (var i in ServersList)
            {
                EmojisServerListModel? server = (i.DataContext as EmojisServerListModel);
                if (server == null)
                    continue;
                EmojiListItem? model = server.EmojisList.FirstOrDefault(x => (x.DataContext as EmojiListItemModel)?.emojiId == emoji.Id);
                if (model == null)
                    continue;

                //model.UpdateLayout();
            }
        });
    }

    private async Task EmojiCreate(RestServer server, RestEmoji emoji)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            EmojisServerList? model = ServersList.FirstOrDefault(x => (x.DataContext as EmojisServerListModel)?.serverId == server.Id);
            if (model == null)
                return;

            (model.DataContext as EmojisServerListModel).EmojisList.Add(new EmojiListItem
            {
                DataContext = new EmojiListItemModel(services, emoji)
            });
        });
    }

    [ObservableProperty]
    private ObservableCollection<EmojisServerList> serversList;
}
