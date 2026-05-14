namespace Guessy.Infrastructure.WebSocket;

public static class ByteCombine
{
    public static byte[] Combine(byte prefix, byte[] data)
    {
        var result = new byte[1 + data.Length];
        result[0] = prefix;
        Buffer.BlockCopy(data, 0, result, 1, data.Length);
        return result;
    }
}