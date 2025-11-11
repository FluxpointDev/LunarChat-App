using System;
using System.Collections.Generic;

namespace LunarChatApp;

public static class TestState
{
    public static List<ServerData> Servers = new List<ServerData>
    {
        new ServerData
        {
            Id = "1",
        }
    };
    public static Dictionary<string, List<ChannelData>> Channels = new Dictionary<string, List<ChannelData>>()
    {
        { "1", new List<ChannelData>
            {
                new ChannelData(),
                new ChannelData
                {
                    Name = "media",
                    Type = ChannelType.Media
                },
                new ChannelData
                {
                    Name = "voice",
                    Type = ChannelType.Voice
                },
                new ChannelData
                {
                    Name = "schedule",
                    Type = ChannelType.Schedule
                }
            }
        }
    };
    public static Dictionary<string, List<MessageData>> Messages = new Dictionary<string, List<MessageData>>
    {
        { "1", new List<MessageData>
            {
                new MessageData(),
                new MessageData()
                {
                    Content = "Hi :)"
                }
            }
        }
    };
}
public class ServerData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Fluxpoint Development";
    public string Icon = "test-icon.ico";
}
public class ChannelData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "general";
    public ChannelType Type { get; set; }
}
public enum ChannelType
{
    Text, Voice, Media, Schedule, Rules
}
public class MessageData
{
    public string Content = "# Hello, Markdown!\n" +
            "https://google.com\n" +
            "- Test 1\n" +
            "- Test 2\n" +
            "**Bold**\n" +
            "```cs\n" +
            "public void Test() {\n" +
            "}\n" +
            "```";

    public UserData User = new UserData();
    public string Time { get; set; } = "10:00 PM";
}
public class UserData
{
    public string Id { get; set; } = "1";
    public string Name { get; set; } = "Builderb";
    public string Avatar { get; set; } = "avatar.png";
}