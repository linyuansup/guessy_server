using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Models;

public class Leaderboard(Dictionary<BraceletId, Score> score)
{
    private readonly Dictionary<BraceletId, Score>
        _scores = score;

    public IReadOnlyDictionary<BraceletId, Score>
        Scores => _scores;
}