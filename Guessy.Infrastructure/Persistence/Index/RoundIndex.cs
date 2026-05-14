using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Index;

public static class RoundIndex
{
    public static async Task CreateAsync(IMongoCollection<RoundMapping> roundCollection,
        CancellationToken cancellationToken)
    {
        await roundCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<RoundMapping>(
                Builders<RoundMapping>.IndexKeys.Ascending(r => r.QuestionId)),
            cancellationToken: cancellationToken);

        await roundCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<RoundMapping>(
                Builders<RoundMapping>.IndexKeys.Ascending(r => r.DrawerId)),
            cancellationToken: cancellationToken);

        await roundCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<RoundMapping>(
                Builders<RoundMapping>.IndexKeys.Descending(r => r.CreatedAt)),
            cancellationToken: cancellationToken);
    }
}