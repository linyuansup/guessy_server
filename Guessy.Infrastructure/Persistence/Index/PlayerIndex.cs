using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Index;

public static class PlayerIndex
{
    public static async Task CreateAsync(IMongoCollection<PlayerMapping> playerCollection,
        CancellationToken cancellationToken)
    {
        await playerCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<PlayerMapping>(
                Builders<PlayerMapping>.IndexKeys.Ascending(p => p.BraceletId)),
            cancellationToken: cancellationToken);

        await playerCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<PlayerMapping>(
                Builders<PlayerMapping>.IndexKeys.Ascending(p => p.DeviceId)),
            cancellationToken: cancellationToken);

        await playerCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<PlayerMapping>(
                Builders<PlayerMapping>.IndexKeys.Ascending(p => p.Score)),
            cancellationToken: cancellationToken);
    }
}