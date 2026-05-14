using Guessy.Domain.Models;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Repositories;

public interface IPlayerRepository
{
    Task UpdateAsync(Player player);
    Task<Player?> GetAsync(BraceletId braceletId);
    Task<Player?> GetAsync(DeviceId deviceId);
    Task<IEnumerable<Player>?> GetAllAsync(int page, int pageSize);
    Task DeleteAsync(BraceletId braceletId);
}