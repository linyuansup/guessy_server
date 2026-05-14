using MongoDB.Bson;

namespace Guessy.Infrastructure.Persistence.Mapping;

public record QuestionMapping(ObjectId Id, List<string> Hints, string Content, DateTimeOffset? UsedAt);