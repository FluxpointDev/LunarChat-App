using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Users;
using LunarChatApp.Shared.WebSocket.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace LunarChatApp.Shared.WebSocket;

public class LunarSocketClient
{
    public LunarSocketClient(string url, string auth)
    {
        webSocketUrl = url;
        authId = auth;
        State.WebSocket = this;
    }

    public delegate void MessageEventHandler(MessageRecievedEvent message);
    public delegate void ServerJoinEventHandler(ServerJoinEvent server);
    public SocketState State = new SocketState();

    public event MessageEventHandler? OnMessageRecieved;
    public void TriggerMessage(MessageRecievedEvent message)
    {
        OnMessageRecieved?.Invoke(message);
    }



    private string webSocketUrl;
    private string authId;
    private bool _firstConnected { get; set; } = true;
    private bool _firstError = true;
    public bool StopWebSocket = false;

    internal ClientWebSocket? WebSocket;
    internal CancellationToken CancellationToken = new CancellationToken();
    internal User? CurrentUser;
    public async Task SetupWebsocket()
    {
        StopWebSocket = false;

        while (!CancellationToken.IsCancellationRequested && !StopWebSocket)
        {
            using (WebSocket = new ClientWebSocket())
            {
                //if (Client.Config.WebSocketProxy != null)
                //    WebSocket.Options.Proxy = Client.Config.WebSocketProxy;

                try
                {
                    Uri uri = new Uri($"{webSocketUrl}?format=json&version=1");

                    //if (!string.IsNullOrEmpty(Client.Config.CfClearance))
                    //{
                    //    WebSocket.Options.Cookies = new System.Net.CookieContainer();
                    //    WebSocket.Options.Cookies.SetCookies(uri, $"cf_clearance={Client.Config.CfClearance}");
                    //}
                    //WebSocket.Options.SetRequestHeader("Origin", "https://lunar.fluxpoint.dev");
                    //WebSocket.Options.SetRequestHeader("User-Agent", "Lunar Client");
                    //WebSocket.Options.SetRequestHeader("Auth-Id", authId);
                    await WebSocket.ConnectAsync(uri, CancellationToken);
                    _ = Send(WebSocket, JsonConvert.SerializeObject(new AuthEvent
                    {
                        user_id = authId
                    }), CancellationToken.None);
                    //TriggerMessage(new MessageRecievedEvent
                    //{
                    //    content = "Online"
                    //});
                    //await Send(WebSocket, JsonConvert.SerializeObject(new AuthenticateSocketRequest(Client.Token)), CancellationToken);
                    _firstError = true;
                    await Receive(WebSocket, CancellationToken);
                }
                catch (ArgumentException)
                {
                    TriggerMessage(new MessageRecievedEvent
                    {
                        content = "Invalid websocket url"
                    });
                    //if (_firstConnected)
                    //    Client.InvokeLogAndThrowException("Client config WebsocketUrl is an invalid format.");
                }
                catch (WebSocketException we)
                {
                    TriggerMessage(new MessageRecievedEvent
                    {
                        content = we.ToString()
                    });
                    if (_firstConnected)
                    {
                        //if (we.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                        //    Client.InvokeLogAndThrowException("Failed to connect to Stoat, the instance may be down or having issues.");

                        //Client.InvokeLogAndThrowException("Failed to connect to Stoat.");
                    }
                    else
                    {
                        //if (we.WebSocketErrorCode != WebSocketError.ConnectionClosedPrematurely)
                        //    Client.InvokeLog($"WebSocket Internal Error {we.ErrorCode} {we.WebSocketErrorCode}", StoatLogSeverity.Error);
                    }

                }
                catch (Exception ex)
                {
                    TriggerMessage(new MessageRecievedEvent
                    {
                        content = ex.ToString()
                    });
                    //Client.InvokeLog($"WebSocket Error {ex.Message}", StoatLogSeverity.Error);
                    //if (_firstConnected)
                    //    Client.InvokeLogAndThrowException("Failed to connect to Stoat.");
                }
                await Task.Delay(_firstError ? 3000 : 10000, CancellationToken);
                _firstError = false;
            }
        }
    }

    internal Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken)
        => socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

    private async Task Receive(ClientWebSocket socket, CancellationToken cancellationToken)
    {
        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
        while (!cancellationToken.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            await using (MemoryStream ms = new MemoryStream())
            {
                do
                {
                    result = await socket.ReceiveAsync(buffer, cancellationToken);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                {
                    _ = WebSocketMessage(await reader.ReadToEndAsync());
                }
            }
        }
    }

    private async Task WebSocketMessage(string json)
    {
        JToken payload = JsonConvert.DeserializeObject<JToken>(json);

        try
        {
            switch (payload["type"].ToString())
            {
                case "auth":
                    if (_firstConnected)
                    {
                        //Client.InvokeConnected();
                        //Client.InvokeLog("WebSocket Connected!", StoatLogSeverity.Debug);
                    }
                    //else
                    //Client.InvokeLog("WebSocket Reconnected!", StoatLogSeverity.Debug);

                    _firstConnected = false;
                    //await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatSocketRequest()), CancellationToken);

                    //_ = Task.Run(async () =>
                    //{
                    //    while (!CancellationToken.IsCancellationRequested)
                    //    {
                    //        await Task.Delay(50000, CancellationToken);
                    //        await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);
                    //    }
                    //}, CancellationToken);
                    break;
                case "ready":
                    {
                        _firstConnected = false;
                        ReadyEvent? data = JsonConvert.DeserializeObject<ReadyEvent>(json);
                        State.Channels = data.channels;
                        State.Friends = data.Friends;
                        State.Blocks = data.Blocks;
                        try
                        {
                            State.Servers = new ConcurrentDictionary<string, SocketServerState>(data.servers.ToDictionary(x => x.Id, x => new SocketServerState
                            {
                                Server = x,
                                Channels = new ConcurrentDictionary<string, Channel>(State.Channels[x.Id].ToDictionary(x => x.Id, x => x))
                            }));
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        Console.WriteLine("Test");
                        foreach (var i in State.Servers.Values)
                        {
                            State.TriggerAddServer(i.Server);
                        }
                    }
                    break;
                case "message_create":
                    {
                        MessageRecievedEvent? data = JsonConvert.DeserializeObject<MessageRecievedEvent>(json);
                        TriggerMessage(data);

                    }
                    break;
                case "message_update":
                    {
                        MessageUpdateEvent? data = JsonConvert.DeserializeObject<MessageUpdateEvent>(json);
                        _ = State.OnMessageEdit?.Invoke(new Core.Messages.Message
                        {
                            AuthorId = data.user.Id,
                            ChannelId = data.channel_id,
                            Content = data.content,
                            Id = data.id,
                            Username = data.user.Username
                        });
                    }
                    break;
                case "message_delete":
                    {
                        MessageDeleteEvent? data = JsonConvert.DeserializeObject<MessageDeleteEvent>(json);
                        _ = State.OnMessageDelete?.Invoke(new Core.Messages.Message
                        {
                            AuthorId = data.user.Id,
                            ChannelId = data.channel_id,
                            Content = data.content,
                            Id = data.id,
                            Username = data.user.Username
                        });
                    }
                    break;
                case "server_join":
                    {
                        ServerJoinEvent? data = JsonConvert.DeserializeObject<ServerJoinEvent>(json);
                        State.Servers.TryAdd(data.server.Id, new SocketServerState
                        {
                            Server = data.server,
                            Channels = new ConcurrentDictionary<string, Channel>(data.channels)
                        });
                        var channels = new List<Channel>();
                        foreach (var i in data.channels)
                        {
                            channels.Add(i.Value);
                        }
                        State.Channels.TryAdd(data.server.Id, channels);
                        State.TriggerAddServer(data.server);
                    }
                    break;

                case "server_left":
                    {
                        ServerLeftEvent? data = JsonConvert.DeserializeObject<ServerLeftEvent>(json);
                        State.TriggerDeleteServer(State.Servers[data.server_id].Server);
                        State.Servers.TryRemove(data.server_id, out _);
                        State.Channels.TryRemove(data.server_id, out _);
                    }
                    break;
                case "channel_create":
                    {
                        ChannelCreatedEvent? data = JsonConvert.DeserializeObject<ChannelCreatedEvent>(json);
                        State.Servers[data.channel.ServerId].Channels.TryAdd(data.channel.Id, data.channel);
                        State.Channels[data.channel.ServerId].Add(data.channel);
                        if (State.CurrentServer.Server.Id == data.channel.ServerId)
                            State.CurrentServer.OnChannelCreate.Invoke(data.channel);
                    }
                    break;
                case "channel_delete":
                    {
                        ChannelDeletedEvent? data = JsonConvert.DeserializeObject<ChannelDeletedEvent>(json);
                        var channel = State.Channels[data.server_id].FirstOrDefault(x => x.Id == data.channel_id);
                        State.Servers[data.server_id].Channels.TryRemove(data.channel_id, out channel);
                        State.Channels[data.server_id].Remove(channel);
                        if (State.CurrentServer.Server.Id == channel.ServerId)
                            State.CurrentServer.OnChannelDelete.Invoke(channel);
                    }
                    break;
                case "account_friend_add":
                    {
                        AccountFriendAdd? data = JsonConvert.DeserializeObject<AccountFriendAdd>(json);
                        State.Friends.Add(data.relation.id, data.relation);
                        State.OnFriendAdd.Invoke(data.relation);
                    }
                    break;
                case "account_friend_remove":
                    {
                        AccountFriendRemove? data = JsonConvert.DeserializeObject<AccountFriendRemove>(json);
                        State.Friends.Remove(data.user_id, out var relation);
                        State.OnFriendRemove.Invoke(relation);
                    }
                    break;
                case "account_block_add":
                    {
                        AccountBlockAdd? data = JsonConvert.DeserializeObject<AccountBlockAdd>(json);
                        State.Blocks.Add(data.relation.id, data.relation);
                        State.OnBlockAdd.Invoke(data.relation);
                    }
                    break;
                case "account_block_remove":
                    {
                        AccountBlockRemove? data = JsonConvert.DeserializeObject<AccountBlockRemove>(json);
                        State.Blocks.Remove(data.user_id, out var relation);
                        State.OnBlockRemove.Invoke(relation);
                    }
                    break;
            }
        }
        catch { }
    }
}
