using LunarChatApp.Shared.Core.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace LunarChatApp.Shared.WebSocket;

public class LunarSocketClient
{
    public LunarSocketClient(string url, string auth)
    {
        webSocketUrl = url;
        authId = auth;
    }
    public delegate void MessageEventHandler(SocketMessageRecieve message);
    public event MessageEventHandler? OnMessageRecieved;
    public void TriggerMessage(SocketMessageRecieve message)
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
                    TriggerMessage(new SocketMessageRecieve
                    {
                        content = "Test"
                    });
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
                    TriggerMessage(new SocketMessageRecieve
                    {
                        content = "Online"
                    });
                    //await Send(WebSocket, JsonConvert.SerializeObject(new AuthenticateSocketRequest(Client.Token)), CancellationToken);
                    _firstError = true;
                    await Receive(WebSocket, CancellationToken);
                }
                catch (ArgumentException)
                {
                    TriggerMessage(new SocketMessageRecieve
                    {
                        content = "Invalid websocket url"
                    });
                    //if (_firstConnected)
                    //    Client.InvokeLogAndThrowException("Client config WebsocketUrl is an invalid format.");
                }
                catch (WebSocketException we)
                {
                    TriggerMessage(new SocketMessageRecieve
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
                    TriggerMessage(new SocketMessageRecieve
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
                case "Authenticated":
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
                case "Message":
                    {
                        SocketMessageRecieve? message = JsonConvert.DeserializeObject<SocketMessageRecieve>(json);
                        TriggerMessage(message);

                    }
                    break;
            }
        }
        catch { }
    }
}
