using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.ValueObjects.Game;

public class AnswerRecord(BraceletId braceletId, string answer, DateTimeOffset submittedAt)
{
    public BraceletId PlayerId { get; private set; } = braceletId;
    public string Answer { get; private set; } = answer;
    public DateTimeOffset SubmittedAt { get; private set; } = submittedAt;
}