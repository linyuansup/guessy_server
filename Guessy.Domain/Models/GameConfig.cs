using Guessy.Domain.ValueObjects.Config;

namespace Guessy.Domain.Models;

public class GameConfig(ScoreConfig scoreConfig, RoundConfig roundConfig, LeaderboardConfig leaderboardConfig)
{
    public ScoreConfig ScoreConfig { get; set; } = scoreConfig;
    public RoundConfig RoundConfig { get; set; } = roundConfig;
    public LeaderboardConfig LeaderboardConfig { get; set; } = leaderboardConfig;

    public static GameConfig Default => new(ScoreConfig.Default, RoundConfig.Default, LeaderboardConfig.Default);
}