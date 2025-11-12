namespace LunarChatApp.Shared.Rest;

public class LunarException : Exception
{
    public LunarException(string message) : base(message)
    {
    }
}
public class LunarArgumentException : LunarException
{
    public LunarArgumentException(string message) : base(message)
    {

    }
}
public class LunarRestException : LunarException
{
    public LunarRestException(string message, int code) : base(message)
    {
        Code = code;
    }
    /// <summary>
    /// The status code error for this exception if thrown by the rest client.
    /// </summary>
    public int Code { get; internal set; }
}
