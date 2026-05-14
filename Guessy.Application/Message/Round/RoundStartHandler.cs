using Guessy.Application.Service;
using Guessy.Domain.Enums;
using Guessy.Domain.Events.Round;
using Guessy.Domain.Repositories;
using MediatR;

namespace Guessy.Application.Message.Round;

public class RoundStartHandler(RoundApplicationService roundService, IQuestionRepository questionRepository)
    : INotificationHandler<RoundStartEvent>
{
    public async Task Handle(RoundStartEvent notification, CancellationToken cancellationToken)
    {
        roundService.StartRound(notification.Round);
        var question = notification.Round.Question;
        question.UseState = QuestionUseState.Used;
        await questionRepository.UpdateAsync(question);
    }
}