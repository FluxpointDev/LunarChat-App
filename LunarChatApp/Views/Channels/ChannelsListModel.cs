using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using LunarChatApp.Views.Channels;
using LunarChatSharp.Core.Channels;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelsListModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    public ChannelsListModel(ServiceManager sv, TestState st)
    {
        state = st;
        services = sv;
        bool CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        if (ChannelsList == null)
        {
            _channelsList = new ObservableCollection<CategoryItem>
            {
                new CategoryItem() { DataContext = new CategoryItemModel(services, new RestChannel
                {
                    Id = null!,
                    Name = null!
                }, CanManage) }
            };
            _channelsList.AddRange(services.State.CurrentServer.Channels.Values.Where(x => x.Type == ChannelType.Category).OrderBy(x => x.Position).Select(x => new CategoryItem
            {
                DataContext = new CategoryItemModel(services, x, CanManage)
            }));
        }
        services.Client.OnChannelUpdate += ChannelUpdate;
        services.Client.OnChannelDelete += ChannelDelete;
        services.Client.OnChannelCreate += Server_OnChannelCreate;
        services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
    }

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        bool CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (var i in ChannelsList)
            {
                CategoryItemModel? model = i.DataContext as CategoryItemModel;
                if (model == null)
                    continue;

                foreach (var c in model.ChannelsList)
                {
                    ChannelItemModel? chanModel = c.DataContext as ChannelItemModel;
                    if (chanModel == null)
                        continue;

                    chanModel.CanManage = CanManage;
                }
            }
        });
    }

    private async Task ChannelDelete(RestChannel channel)
    {
        if (services.State.CurrentServer == null || services.State.CurrentServer.Server?.Id != channel.ServerId)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (channel.Type == ChannelType.Category)
            {

            }
            else
            {

            }
            foreach (var i in ChannelsList)
            {
                CategoryItemModel? model = i.DataContext as CategoryItemModel;
                if (model == null)
                    continue;

                ChannelItem? item = model.ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemModel)?.id == channel.Id);
                if (item == null)
                    continue;

                model.ChannelsList.Remove(item);
                break;
            }
        });

    }

    private async Task ChannelUpdate(RestChannel channel, UpdateChannelRequest request)
    {
        if (services.State.CurrentServer == null || services.State.CurrentServer.Server?.Id != channel.ServerId)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (channel.Type == ChannelType.Category)
            {

            }
            else
            {
                foreach (var i in ChannelsList)
                {
                    CategoryItemModel? model = i.DataContext as CategoryItemModel;
                    if (model == null)
                        continue;

                    ChannelItemModel? item = model.ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemModel)?.id == channel.Id)?.DataContext as ChannelItemModel;
                    if (item == null)
                        continue;

                    item.Name = channel.Name;
                }
            }
        });
    }

    private async Task Server_OnChannelCreate(RestChannel channel)
    {
        if (services.State.CurrentServer == null || services.State.CurrentServer.Server?.Id != channel.ServerId)
            return;

        bool CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (channel.Type == ChannelType.Category)
            {
                ChannelsList.Add(new CategoryItem { DataContext = new CategoryItemModel(services, channel, CanManage) });
            }
            else
            {
                CategoryItemModel? model = ChannelsList.FirstOrDefault(x => (x.DataContext as CategoryItemModel)?.id == channel.ParentId)?.DataContext as CategoryItemModel;
                if (model == null)
                    return;

                model.ChannelsList.Add(new ChannelItem { DataContext = new ChannelItemModel(services, state, channel, CanManage) });
            }
        });

    }

    [ObservableProperty]
    private ObservableCollection<CategoryItem> _channelsList;
}
