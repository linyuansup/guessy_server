using MediatR;

namespace Guessy.Domain.Events.Round;

public record RoundStartEvent(Models.Round Round) : INotification;