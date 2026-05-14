using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Index;

public static class QuestionIndex
{
    public static async Task CreateOneAsync(IMongoCollection<QuestionMapping> questionCollection,
        CancellationToken cancellationToken)
    {
        await questionCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<QuestionMapping>(
                Builders<QuestionMapping>.IndexKeys.Ascending(q => q.UsedAt)),
            cancellationToken: cancellationToken);
    }
}