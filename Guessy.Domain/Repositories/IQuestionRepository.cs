using Guessy.Domain.Models;

namespace Guessy.Domain.Repositories;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>?> GetUnusedAsync(int count);
    Task<IEnumerable<Question>?> GetAsync(int page, int pageSize);
    Task<Question?> GetAsync(string content);
    Task UpdateAsync(Question question);
    Task DeleteAsync(string content);
}