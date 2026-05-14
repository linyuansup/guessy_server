namespace Guessy.Domain.ValueObjects.Game.RoundState;

public record Playing(DateTimeOffset StartTime) : IRoundState;