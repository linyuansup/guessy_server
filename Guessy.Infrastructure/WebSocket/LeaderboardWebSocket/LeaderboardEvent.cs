namespace Guessy.Infrastructure.WebSocket.LeaderboardWebSocket;

public enum LeaderboardEvent : byte
{
    LeaderboardUpdated = 1,
    GamePrepared = 2,
    GameStopped = 3,
    TimeConfigChanged = 4,
    LeaderboardConfigChanged = 5,
    AddAnswer = 6,
    GameStart = 7
}