using Guessy.Domain.Events.Round;
using Guessy.Infrastructure.WebSocket;
using MediatR;

namespace Guessy.Application.Message.Round;

public class RoundPrepareHandler(WebSocketManager webSocketManager) : INotificationHandler<RoundPrepareEvent>
{
    public async Task Handle(RoundPrepareEvent notification, CancellationToken cancellationToken)
    {
        webSocketManager.LeaderboardWebsocket?.SendGamePrepared();
        webSocketManager.DrawerWebsocket?.SendGamePrepared(notification.questions.Select(question => question.Content).ToList());
    }
}