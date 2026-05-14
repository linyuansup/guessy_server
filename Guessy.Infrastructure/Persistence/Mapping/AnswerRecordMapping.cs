using MongoDB.Bson;

namespace Guessy.Infrastructure.Persistence.Mapping;

public record AnswerRecordMapping(
    ObjectId Id,
    ObjectId PlayerId,
    ObjectId QuestionId,
    string Answer,
    DateTimeOffset SubmittedAt);