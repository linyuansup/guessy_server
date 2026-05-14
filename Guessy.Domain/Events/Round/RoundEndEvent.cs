using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;
using MediatR;

namespace Guessy.Domain.Events.Round;

public record RoundEndEvent(Dictionary<BraceletId, Score> Rank) : INotification;