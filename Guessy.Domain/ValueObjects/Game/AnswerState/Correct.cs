namespace Guessy.Domain.ValueObjects.Game.AnswerState;

public record Correct(int Rank, Score Score) : IAnswerState;