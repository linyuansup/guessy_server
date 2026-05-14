using Guessy.Domain.Models;
using Guessy.Domain.Services.Interfaces;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Services;

public class ScoreCalculator(IGameConfigService configService) : IScoreCalculator
{
    public Dictionary<BraceletId, Score> CalculateScore(Round round)
    {
        var rewardsByRank = configService.Config.ScoreConfig.RankRewards
            .ToDictionary(reward => reward.Rank);
        Dictionary<BraceletId, int> scores = new();
        foreach (var (answer, index) in round.CorrectAnswers
                     .OrderBy(answer => answer.SubmittedAt)
                     .Select((answer, index) => (answer, index)))
        {
            var rank = index + 1;
            if (!rewardsByRank.TryGetValue(rank, out var reward))
            {
                continue;
            }

            AddScore(scores, answer.PlayerId, reward.PlayerScore.Value);
            AddScore(scores, round.DrawerId, reward.DrawerScore.Value);
        }

        return scores.ToDictionary(pair => pair.Key, pair => new Score(pair.Value));
    }

    private static void AddScore(Dictionary<BraceletId, int> scores, BraceletId braceletId, int value)
    {
        if (value == 0)
        {
            return;
        }

        if (scores.TryGetValue(braceletId, out var currentScore))
        {
            scores[braceletId] = currentScore + value;
            return;
        }

        scores[braceletId] = value;
    }
}