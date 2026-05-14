using Guessy.Domain.Enums;
using Guessy.Domain.ValueObjects.Game;

namespace Guessy.Domain.Models;

public class Question(string content, IEnumerable<Hint> hints)
{
    private readonly List<Hint> _hints = hints.ToList();
    public QuestionUseState UseState { get; set; }
        = QuestionUseState.Unused;
    public string Content { get; private set; } = content;

    public IReadOnlyList<Hint> Hints => _hints;
}