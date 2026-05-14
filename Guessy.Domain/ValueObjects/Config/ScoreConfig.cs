namespace Guessy.Domain.ValueObjects.Config;

public record ScoreConfig(IEnumerable<RankReward> RankRewards)
{
    public static ScoreConfig Default => new([
        new(1, new(5), new(1)), new(2, new(3), new(1)),
        new(3, new(1), new(1))
    ]);
}