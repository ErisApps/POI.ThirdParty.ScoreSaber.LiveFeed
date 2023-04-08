using System.Text.Json.Serialization;
using POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Scores;

namespace POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Websocket;

public class ScoreCommand : Command<PlayerScore>
{
    [JsonConstructor]
    public ScoreCommand(string commandName, PlayerScore commandData) : base(commandName, commandData)
    {
    }
}