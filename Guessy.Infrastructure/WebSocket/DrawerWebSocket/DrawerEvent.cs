namespace Guessy.Infrastructure.WebSocket.DrawerWebSocket;

public enum DrawerEvent : byte
{
    GamePrepared = 1,
    GameStopped = 2,
    TimeConfigChanged = 3
}