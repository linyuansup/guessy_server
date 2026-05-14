using Guessy.Domain.Events.Round;
using Guessy.Domain.Repositories;
using Guessy.Infrastructure.WebSocket;
using MediatR;

namespace Guessy.Application.Message.Round;

public class RoundEndHandler(WebSocketManager webSocketManager, IPlayerRepository playerRepository) : INotificationHandler<RoundEndEvent>
{
    public async Task Handle(RoundEndEvent notification, CancellationToken cancellationToken)
    {
        foreach (var (braceletId, score) in notification.Rank)
        {
            var player = await playerRepository.GetAsync(braceletId);
            if (player == null)
            {
                continue;
            }

            player.AddScore(score.Value);
            await playerRepository.UpdateAsync(player);
        }

        var leaderboard = webSocketManager.LeaderboardWebsocket;
        var drawer = webSocketManager.DrawerWebsocket;
        if (leaderboard != null)
        {
            await leaderboard.SendGameStopped(new(notification.Rank));
        }
        if (drawer != null)
        {
            await drawer.SendGameStopped();
        }
    }
}