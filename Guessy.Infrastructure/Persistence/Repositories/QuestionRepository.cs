using Guessy.Domain.Enums;
using Guessy.Domain.Models;
using Guessy.Domain.Repositories;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Infrastructure.Persistence.DbContext;
using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Repositories;

public class QuestionRepository(MongoContext db) : IQuestionRepository
{
    public async Task<IEnumerable<Question>?> GetUnusedAsync(int count)
    {
        if (count <= 0)
        {
            return null;
        }

        var mappings = await db.QuestionCollection
            .Find(Builders<QuestionMapping>.Filter.Eq(q => q.UsedAt, null))
            .ToListAsync();

        if (mappings.Count == 0)
        {
            return null;
        }

        var shuffled = mappings
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .Select(MapToDomain)
            .ToList();

        return shuffled.Count == 0 ? null : shuffled;
    }

    public async Task<IEnumerable<Question>?> GetAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var mappings = await db.QuestionCollection
            .Find(Builders<QuestionMapping>.Filter.Empty)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        if (mappings == null || mappings.Count == 0)
        {
            return null;
        }

        return mappings.Select(MapToDomain).ToList();
    }

    public async Task<Question?> GetAsync(string content)
    {
        var filter = Builders<QuestionMapping>.Filter.Eq(q => q.Content, content);
        var mapping = await db.QuestionCollection.Find(filter).FirstOrDefaultAsync();
        return mapping is null ? null : MapToDomain(mapping);
    }

    public async Task UpdateAsync(Question question)
    {
        var filter = Builders<QuestionMapping>.Filter.Eq(q => q.Content, question.Content);
        var existing = await db.QuestionCollection.Find(filter).FirstOrDefaultAsync();
        var usedAt = question.UseState == QuestionUseState.Used ? DateTimeOffset.UtcNow : (DateTimeOffset?)null;

        if (existing is not null)
        {
            var update = Builders<QuestionMapping>.Update
                .Set(q => q.Hints, question.Hints.Select(h => h.Value).ToList())
                .Set(q => q.UsedAt, usedAt);

            await db.QuestionCollection.UpdateOneAsync(filter, update);
        }
        else
        {
            var mapping = new QuestionMapping(
                ObjectId.GenerateNewId(),
                question.Hints.Select(h => h.Value).ToList(),
                question.Content,
                usedAt);

            await db.QuestionCollection.InsertOneAsync(mapping);
        }
    }

    public async Task DeleteAsync(string content)
    {
        var filter = Builders<QuestionMapping>.Filter.Eq(q => q.Content, content);
        await db.QuestionCollection.DeleteOneAsync(filter);
    }

    private static Question MapToDomain(QuestionMapping mapping)
    {
        var hints = mapping.Hints.Select(h => new Hint(h));
        var question = new Question(mapping.Content, hints);

        // Set UseState using reflection since it has a private setter
        if (mapping.UsedAt.HasValue)
        {
            var useStateProperty = typeof(Question).GetProperty(nameof(Question.UseState));
            useStateProperty?.SetValue(question, QuestionUseState.Used);
        }

        return question;
    }
}