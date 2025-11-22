namespace LunarChatApp.Shared.WebSocket;

public class SocketMessage
{
    public string type;
}
public class SocketMessageRecieve : SocketMessage
{
    public string channel_id;
    public string username;
    public string content;
}
