using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Rest.Accounts;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsSocialModel : ViewModelBase
{
    private ServiceManager services;
    public SettingsSocialModel(ServiceManager sv)
    {
        services = sv;
        friendsEveryone = services.State.Socket.Account.FriendRequestAccess.Everyone;
        FriendsMutualServers = services.State.Socket.Account.FriendRequestAccess.MutualServers;
        friendsMutualFriends = services.State.Socket.Account.FriendRequestAccess.MutualFriends;

        messagesEveryone = services.State.Socket.Account.DirectMessagesAccess.Everyone;
        messagesMutualServers = services.State.Socket.Account.DirectMessagesAccess.MutualServers;
        messagesMutualFriends = services.State.Socket.Account.DirectMessagesAccess.MutualFriends;
        PropertyChanged += SettingsSocialModel_PropertyChanged;
    }

    private void SettingsSocialModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        EditAccountRequest request = new EditAccountRequest();
        switch (e.PropertyName)
        {
            case "FriendsEveryone":
                request.FriendRequestAccess = new EditFriendRequestAccess
                {
                    Everyone = FriendsEveryone
                };
                break;
            case "FriendsMutualServers":
                request.FriendRequestAccess = new EditFriendRequestAccess
                {
                    MutualServers = FriendsMutualServers
                };
                break;
            case "FriendsMutualFriends":
                request.FriendRequestAccess = new EditFriendRequestAccess
                {
                    MutualFriends = FriendsMutualFriends
                };
                break;
            case "MessagesEveryone":
                request.DirectMessagesAccess = new EditDirectMessagesAccess
                {
                    Everyone = MessagesEveryone
                };
                break;
            case "MessagesMutualServers":
                request.DirectMessagesAccess = new EditDirectMessagesAccess
                {
                    MutualServers = MessagesMutualServers
                };
                break;
            case "MessagesMutualFriends":
                request.DirectMessagesAccess = new EditDirectMessagesAccess
                {
                    MutualFriends = MessagesMutualFriends
                };
                break;
        }
        if (request.DirectMessagesAccess == null && request.FriendRequestAccess == null)
            return;

        _ = services.Rest.AccountEdit(request);
    }

    [ObservableProperty]
    private bool friendsEveryone;

    [ObservableProperty]
    private bool friendsMutualServers;

    [ObservableProperty]
    private bool friendsMutualFriends;

    [ObservableProperty]
    private bool messagesEveryone;

    [ObservableProperty]
    private bool messagesMutualServers;

    [ObservableProperty]
    private bool messagesMutualFriends;
}
