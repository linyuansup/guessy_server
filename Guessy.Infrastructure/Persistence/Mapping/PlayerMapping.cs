using MongoDB.Bson;

namespace Guessy.Infrastructure.Persistence.Mapping;

public record PlayerMapping(ObjectId Id, string BraceletId, string DeviceId, int Score);