namespace Guessy.Infrastructure.WebSocket;

public class WebSocketManager
{
    public LeaderboardWebSocket.LeaderboardWebSocket? LeaderboardWebsocket
    {
        get;
        set
        {
            field?.Dispose();
            field = value;
        }
    }

    public DrawerWebSocket.DrawerWebSocket? DrawerWebsocket
    {
        get;
        set
        {
            field?.Dispose();
            field = value;
        }
    }
}