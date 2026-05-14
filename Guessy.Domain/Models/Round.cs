using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Game.RoundState;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Models;

public class Round(Question question, BraceletId drawerId)
{
    private readonly List<AnswerRecord> _correctAnswers = [];
    private readonly List<AnswerRecord> _incorrectAnswers = [];
    public Question Question { get; private set; } = question;
    public BraceletId DrawerId { get; private set; } = drawerId;
    public DrawingBoard DrawingBoard { get; set; } = new();
    public IRoundState State { get; private set; } = new Selecting();

    public IReadOnlyList<AnswerRecord> Answers => CorrectAnswers.Concat(IncorrectAnswers).ToList();
    public IReadOnlyList<AnswerRecord> CorrectAnswers => _correctAnswers;
    public IReadOnlyList<AnswerRecord> IncorrectAnswers => _incorrectAnswers;

    public void AddCorrectAnswer(AnswerRecord record)
    {
        _correctAnswers.Add(record);
    }

    public void AddIncorrectAnswer(AnswerRecord record)
    {
        _incorrectAnswers.Add(record);
    }
}