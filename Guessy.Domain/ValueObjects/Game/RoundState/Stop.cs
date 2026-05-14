namespace Guessy.Domain.ValueObjects.Game.RoundState;

public record Stop(DateTimeOffset StartAt, DateTimeOffset EndAt) : IRoundState;