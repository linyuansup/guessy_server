using Google.Protobuf;
using Guessy.Contracts.Proto.Drawer;
using System.Net.WebSockets;

namespace Guessy.Infrastructure.WebSocket.DrawerWebSocket;

public class DrawerWebSocket(System.Net.WebSockets.WebSocket webSocket) : IDisposable
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

    public async Task SendGamePrepared(List<string> questions)
    {
        var proto = new GamePrepared { Questions = { questions } };
        await webSocket.SendAsync(ByteCombine.Combine((byte)DrawerEvent.GamePrepared, proto.ToByteArray()),
            WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async Task SendGameStopped()
    {
        await webSocket.SendAsync(new[] { (byte)DrawerEvent.GameStopped }, WebSocketMessageType.Binary,
            true, CancellationToken.None);
    }

    public async Task SendTimeConfigChanged(int time)
    {
        var proto = new TimeConfigChanged { RoundTime = time };
        await webSocket.SendAsync(ByteCombine.Combine((byte)DrawerEvent.TimeConfigChanged, proto.ToByteArray()),
            WebSocketMessageType.Binary, true, CancellationToken.None);
    }
}