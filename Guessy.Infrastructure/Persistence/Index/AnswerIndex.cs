using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Index;

public static class AnswerIndex
{
    public static async Task CreateAsync(IMongoCollection<AnswerRecordMapping> answerCollection,
        CancellationToken cancellationToken)
    {
        await answerCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<AnswerRecordMapping>(
                Builders<AnswerRecordMapping>.IndexKeys.Ascending(a => a.PlayerId)),
            cancellationToken: cancellationToken);

        await answerCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<AnswerRecordMapping>(
                Builders<AnswerRecordMapping>.IndexKeys.Ascending(a => a.QuestionId)),
            cancellationToken: cancellationToken);

        await answerCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<AnswerRecordMapping>(
                Builders<AnswerRecordMapping>.IndexKeys.Ascending(a => a.SubmittedAt)),
            cancellationToken: cancellationToken);
    }
}