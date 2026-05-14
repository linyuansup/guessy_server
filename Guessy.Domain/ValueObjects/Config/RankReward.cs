using Guessy.Domain.ValueObjects.Game;

namespace Guessy.Domain.ValueObjects.Config;

public record RankReward(int Rank, Score PlayerScore, Score DrawerScore);