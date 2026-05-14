namespace Guessy.Domain.ValueObjects.Config;

public record LeaderboardConfig(int DisplayTopCount)
{
    public static LeaderboardConfig Default => new(10);
}