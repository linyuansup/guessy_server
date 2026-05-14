using Guessy.Application.Service;
using Guessy.Domain.Events.Answer;
using Guessy.Infrastructure.WebSocket;
using MediatR;

namespace Guessy.Application.Message.Answer;

public class AnswerSubmittedHandler(RoundApplicationService roundService, WebSocketManager webSocketManager)
    : INotificationHandler<AnswerSubmittedEvent>
{
    public async Task Handle(AnswerSubmittedEvent notification, CancellationToken cancellationToken)
    {
        roundService.SubmitAnswer(notification.Answer, notification.PlayerId);
    }
}