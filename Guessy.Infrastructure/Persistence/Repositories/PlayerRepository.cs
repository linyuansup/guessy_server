using Guessy.Domain.Models;
using Guessy.Domain.Repositories;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;
using Guessy.Infrastructure.Persistence.DbContext;
using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Repositories;

public class PlayerRepository(MongoContext db) : IPlayerRepository
{
    public async Task UpdateAsync(Player player)
    {
        var filter = Builders<PlayerMapping>.Filter.Eq(p => p.BraceletId, player.BraceletId.Value);
        var existing = await db.PlayerCollection.Find(filter).FirstOrDefaultAsync();

        if (existing is not null)
        {
            var update = Builders<PlayerMapping>.Update
                .Set(p => p.DeviceId, player.DeviceId.Value)
                .Set(p => p.Score, player.Score.Value);

            await db.PlayerCollection.UpdateOneAsync(filter, update);
            return;
        }

        var mapping = new PlayerMapping(
            ObjectId.GenerateNewId(),
            player.BraceletId.Value,
            player.DeviceId.Value,
            player.Score.Value);

        await db.PlayerCollection.InsertOneAsync(mapping);
    }

    public async Task<Player?> GetAsync(BraceletId braceletId)
    {
        var filter = Builders<PlayerMapping>.Filter.Eq(p => p.BraceletId, braceletId.Value);
        var mapping = await db.PlayerCollection.Find(filter).FirstOrDefaultAsync();

        if (mapping == null)
        {
            return null;
        }

        return MapToDomain(mapping);
    }

    public async Task<Player?> GetAsync(DeviceId deviceId)
    {
        var filter = Builders<PlayerMapping>.Filter.Eq(p => p.DeviceId, deviceId.Value);
        var mapping = await db.PlayerCollection.Find(filter).FirstOrDefaultAsync();

        if (mapping == null)
        {
            return null;
        }

        return MapToDomain(mapping);
    }

    public async Task<IEnumerable<Player>?> GetAllAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var mappings = await db.PlayerCollection
            .Find(Builders<PlayerMapping>.Filter.Empty)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        if (mappings == null || mappings.Count == 0)
        {
            return null;
        }

        return mappings.Select(MapToDomain).ToList();
    }

    public async Task DeleteAsync(BraceletId braceletId)
    {
        var filter = Builders<PlayerMapping>.Filter.Eq(p => p.BraceletId, braceletId.Value);
        await db.PlayerCollection.DeleteOneAsync(filter);
    }

    private static Player MapToDomain(PlayerMapping mapping)
    {
        var player = new Player(
            new(mapping.BraceletId),
            new(mapping.DeviceId));

        // Set Score using reflection since it has a private setter
        var scoreProperty = typeof(Player).GetProperty(nameof(Player.Score));
        scoreProperty?.SetValue(player, new Score(mapping.Score));

        return player;
    }
}