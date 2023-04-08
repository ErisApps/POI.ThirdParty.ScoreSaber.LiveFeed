using System.Text.Json.Serialization;

namespace POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Websocket;

public class Command<TData>
{
    [JsonPropertyName("commandName")]
    public string CommandName { get; }

    [JsonPropertyName("commandData")]
    public TData CommandData { get; }

    [JsonConstructor]
    public Command(string commandName, TData commandData)
    {
        CommandName = commandName;
        CommandData = commandData;
    }
}