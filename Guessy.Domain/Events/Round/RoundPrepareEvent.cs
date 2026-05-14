using Guessy.Domain.Models;
using Guessy.Domain.ValueObjects.Ids;
using MediatR;

namespace Guessy.Domain.Events.Round;

public record RoundPrepareEvent(BraceletId drawerId, IReadOnlyList<Question> questions) : INotification;