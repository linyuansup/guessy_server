using Guessy.Domain.Models;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Repositories;

public interface IRoundArchiveRepository
{
    Task UpdateAsync(Round round);
    Task<IEnumerable<Round>?> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<Round>?> GetAllAsync(string question);
    Task<IEnumerable<Round>?> GetAllAsync(BraceletId drawerId);
}