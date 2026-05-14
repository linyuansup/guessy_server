using Guessy.Application.Service;
using Guessy.Domain.Events.Answer;
using Guessy.Infrastructure.WebSocket;
using MediatR;

namespace Guessy.Application.Message.Answer;

public class AnswerWrongHandler(RoundApplicationService roundService, WebSocketManager webSocketManager) : INotificationHandler<AnswerWrongEvent>
{
    public async Task Handle(AnswerWrongEvent notification, CancellationToken cancellationToken)
    {
        if (webSocketManager.LeaderboardWebsocket == null)
        {
            return;
        }
        await webSocketManager.LeaderboardWebsocket.AddAnswer(notification.Answer.PlayerId, notification.Answer.Answer);
        roundService.AddWrongAnswer(notification.Answer);
        return;
    }
}