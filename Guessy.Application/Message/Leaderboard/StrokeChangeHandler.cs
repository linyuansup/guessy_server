using Guessy.Application.Service;
using Guessy.Domain.Events.Leaderboard;
using Guessy.Infrastructure.WebSocket;
using MediatR;

namespace Guessy.Application.Message.Leaderboard;

public class StrokeChangeHandler(WebSocketManager webSocketManager, RoundApplicationService roundApplicationService) : INotificationHandler<StrokeChangeEvent>
{
    public async Task Handle(StrokeChangeEvent notification, CancellationToken cancellationToken)
    {
        roundApplicationService.SetStroke(notification.Stroke);
        var leaderboard = webSocketManager.LeaderboardWebsocket;
        if (leaderboard == null)
        {
            return;
        }

        await leaderboard.SendLeaderboardUpdated(notification.Stroke);
    }
}