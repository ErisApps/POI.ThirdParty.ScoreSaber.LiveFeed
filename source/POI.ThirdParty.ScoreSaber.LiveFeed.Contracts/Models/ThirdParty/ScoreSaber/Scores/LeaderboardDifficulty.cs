using System.Text.Json.Serialization;
using POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.Shared;

namespace POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Scores
{
	public readonly struct LeaderboardDifficulty
	{
		[JsonPropertyName("leaderboardId")]
		public int LeaderboardId { get; }

		[JsonPropertyName("gameMode")]
		public string GameMode { get; }

		[JsonPropertyName("difficulty")]
		public Difficulty Difficulty { get; }

		[JsonPropertyName("difficultyRaw")]
		public string DifficultyRaw { get; }

		[JsonConstructor]
		public LeaderboardDifficulty(int leaderboardId, string gameMode, Difficulty difficulty, string difficultyRaw)
		{
			LeaderboardId = leaderboardId;
			GameMode = gameMode;
			Difficulty = difficulty;
			DifficultyRaw = difficultyRaw;
		}
	}
}