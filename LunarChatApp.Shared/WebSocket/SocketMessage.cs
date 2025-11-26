namespace LunarChatApp.Shared.WebSocket;

public class SocketMessage
{
    public SocketMessage(string tp)
    {
        type = tp;
    }
    public string type;
}
