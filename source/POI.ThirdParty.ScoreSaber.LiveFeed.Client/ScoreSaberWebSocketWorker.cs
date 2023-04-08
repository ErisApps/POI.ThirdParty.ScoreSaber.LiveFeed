using POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Websocket;

namespace POI.ThirdParty.ScoreSaber.LiveFeed.Client
{
	internal sealed class ScoreSaberWebSocketWorker : BaseScoreSaberWebSocketWorker
	{
		protected override async Task HandleScoreCommand(ScoreCommand command)
		{
			static string PrettyPrintScoreCommand(ScoreCommand command)
			{
				var data = command.CommandData;
				return $"Command {command.CommandName}: Player {data.Score.LeaderboardPlayer.Name} set a score of {data.Score.ModifiedScore} on {data.Leaderboard.SongName} by {data.Leaderboard.SongAuthorName} with {Math.Round(data.Score.ModifiedScore * 100d / data.Leaderboard.MaxScore, 2)}%";
			}

			await Console.Out.WriteLineAsync(PrettyPrintScoreCommand(command));
		}
	}
}