using Guessy.Domain.Repositories;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Application.Service;

public class PlayerService(IPlayerRepository playerRepository)
{
    public async Task<bool> Register(string bracelet, string deviceId)
    {
        var player = await playerRepository.GetAsync(new BraceletId(bracelet));
        if (player != null)
        {
            return player.DeviceId.Value == deviceId;
        }

        await playerRepository.UpdateAsync(new(new(bracelet), new(deviceId)));
        return true;
    }
}