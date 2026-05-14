using Guessy.Domain.Models;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Services.Interfaces;

public interface IScoreCalculator
{
    Dictionary<BraceletId, Score> CalculateScore(Round round);
}