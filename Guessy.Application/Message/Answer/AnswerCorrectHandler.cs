using Guessy.Application.Service;
using Guessy.Domain.Events.Answer;
using Guessy.Domain.Services.Interfaces;
using Guessy.Infrastructure.WebSocket;
using MediatR;

namespace Guessy.Application.Message.Answer;

public class AnswerCorrectHandler(RoundApplicationService roundService, IGameConfigService gameConfigService,WebSocketManager webSocketManager)
    : INotificationHandler<AnswerCorrectEvent>
{
    public async Task Handle(AnswerCorrectEvent notification, CancellationToken cancellationToken)
    {
        if (roundService.CorrectAnswers().Any(record => record.PlayerId == notification.Answer.PlayerId))
        {
            return;
        }
        var leaderboard = webSocketManager.LeaderboardWebsocket;
        if (leaderboard != null)
        {
            await leaderboard.AddAnswer(notification.Answer.PlayerId, notification.Answer.Answer);
        }
        roundService.AddCorrectAnswer(notification.Answer);
        var maxCorrectAnswer = gameConfigService.Config.ScoreConfig.RankRewards.Count();
        if (maxCorrectAnswer <= roundService.CorrectAnswers().Count)
        {
            await roundService.EndRound();
        }
    }
}