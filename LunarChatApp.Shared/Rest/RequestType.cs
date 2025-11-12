namespace LunarChatApp.Shared.Rest;

public enum RequestType
{
    /// <summary>
    /// Get data from the API.
    /// </summary>
    Get,
    /// <summary>
    /// Post new messages or create channels.
    /// </summary>
    Post,
    /// <summary>
    /// Delete a message, channel, ect.
    /// </summary>
    Delete,
    /// <summary>
    /// Update an existing channel, server, ect.
    /// </summary>
    Patch,
    /// <summary>
    /// Post new emojis.
    /// </summary>
    Put
}