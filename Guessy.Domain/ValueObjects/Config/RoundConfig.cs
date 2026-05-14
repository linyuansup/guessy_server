namespace Guessy.Domain.ValueObjects.Config;

public record RoundConfig(TimeSpan Duration, int SelectQuestionCount)
{
    public static RoundConfig Default => new(TimeSpan.FromSeconds(90), 3);
}