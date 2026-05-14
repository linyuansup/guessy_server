using MongoDB.Bson;

namespace Guessy.Infrastructure.Persistence.Mapping;

public record RoundMapping(
    ObjectId Id,
    ObjectId QuestionId,
    ObjectId DrawerId,
    List<AnswerRecordMapping> CorrectAnswers,
    List<AnswerRecordMapping> IncorrectAnswers,
    string DrawingBoardSchema,
    DateTimeOffset CreatedAt);