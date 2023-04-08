using System.Text.Json.Serialization;
using POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Websocket;

namespace POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Helpers.Json;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(ScoreCommand))]
public partial class ScoreSaberSerializerContext : JsonSerializerContext
{
}