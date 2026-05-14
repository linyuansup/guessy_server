using Google.Protobuf;
using Guessy.Contracts.Proto.Leaderboard;
using Guessy.Domain.Models;
using Guessy.Domain.ValueObjects.Ids;
using System.Net.WebSockets;

namespace Guessy.Infrastructure.WebSocket.LeaderboardWebSocket;

public class LeaderboardWebSocket(System.Net.WebSockets.WebSocket webSocket) : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        webSocket.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public async Task SendLeaderboardUpdated(string leaderboard)
    {
        var proto = new LeaderboardUpdated { Leaderboard = leaderboard };
        await webSocket.SendAsync(ByteCombine.Combine((byte)LeaderboardEvent.LeaderboardUpdated, proto.ToByteArray()),
            WebSocketMessageType.Binary,
            true, CancellationToken.None);
    }

    public async Task SendGamePrepared()
    {
        await webSocket.SendAsync(new[] { (byte)LeaderboardEvent.GamePrepared },
            WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async Task SendGameStopped(Leaderboard leaderboard)
    {
        await webSocket.SendAsync(
            ByteCombine.Combine((byte)LeaderboardEvent.GameStopped,
                new GameStopped
                {
                    Scores =
                    {
                        leaderboard.Scores.Select(score =>
                            new ScoreItem { Score = score.Value.Value, Player = score.Key.Value })
                    }
                }.ToByteArray()),
            WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async Task SendTimeConfigChanged(int roundTime)
    {
        var proto = new TimeConfigChanged { RoundTime = roundTime };
        await webSocket.SendAsync(ByteCombine.Combine((byte)LeaderboardEvent.TimeConfigChanged, proto.ToByteArray()),
            WebSocketMessageType.Binary,
            true, CancellationToken.None);
    }

    public async Task SendLeaderboardConfigChanged(int showFirstRank)
    {
        var proto = new LeaderboardConfigChanged { ShowFirstRank = showFirstRank };
        await webSocket.SendAsync(
            ByteCombine.Combine((byte)LeaderboardEvent.LeaderboardConfigChanged, proto.ToByteArray()),
            WebSocketMessageType.Binary,
            true, CancellationToken.None);
    }

    public async Task AddAnswer(BraceletId user, string answer)
    {
        var proto = new AddAnswer { Player = user.Value, Answer = answer };
        await webSocket.SendAsync(ByteCombine.Combine((byte)LeaderboardEvent.AddAnswer, proto.ToByteArray()),
            WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async Task SendGameStart(Domain.Models.Question question)
    {
        var proto = new Contracts.Proto.Leaderboard.Question
        {
            Content = question.Content,
            Hints = { question.Hints.Select(p => p.Value) }
        };
        await webSocket.SendAsync(ByteCombine.Combine((byte)LeaderboardEvent.GameStart, proto.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
    }
}