using MediatR;

namespace Guessy.Domain.Events.Leaderboard;

public record StrokeChangeEvent(string Stroke) : INotification;