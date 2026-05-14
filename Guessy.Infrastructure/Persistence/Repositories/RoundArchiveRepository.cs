using Guessy.Domain.Enums;
using Guessy.Domain.Models;
using Guessy.Domain.Repositories;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;
using Guessy.Infrastructure.Persistence.DbContext;
using Guessy.Infrastructure.Persistence.Mapping;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.Repositories;

public class RoundArchiveRepository(MongoContext db) : IRoundArchiveRepository
{
    public async Task UpdateAsync(Round round)
    {
        var question = await db.QuestionCollection
            .Find(q => q.Content == round.Question.Content)
            .FirstOrDefaultAsync();

        if (question is null)
        {
            throw new InvalidOperationException($"Question '{round.Question.Content}' not found");
        }

        var drawerPlayer = await db.PlayerCollection
            .Find(p => p.BraceletId == round.DrawerId.Value)
            .FirstOrDefaultAsync();

        if (drawerPlayer is null)
        {
            throw new InvalidOperationException($"Player with BraceletId '{round.DrawerId.Value}' not found");
        }

        // Map answers
        var correctMappings = new List<AnswerRecordMapping>();
        foreach (var ans in round.CorrectAnswers)
        {
            var player = await db.PlayerCollection
                .Find(p => p.BraceletId == ans.PlayerId.Value)
                .FirstOrDefaultAsync();

            if (player is null)
            {
                throw new InvalidOperationException($"Player with BraceletId '{ans.PlayerId.Value}' not found");
            }

            correctMappings.Add(new AnswerRecordMapping(
                ObjectId.GenerateNewId(),
                player.Id,
                question.Id,
                ans.Answer,
                ans.SubmittedAt));
        }

        var incorrectMappings = new List<AnswerRecordMapping>();
        foreach (var ans in round.IncorrectAnswers)
        {
            var player = await db.PlayerCollection
                .Find(p => p.BraceletId == ans.PlayerId.Value)
                .FirstOrDefaultAsync();

            if (player is null)
            {
                throw new InvalidOperationException($"Player with BraceletId '{ans.PlayerId.Value}' not found");
            }

            incorrectMappings.Add(new AnswerRecordMapping(
                ObjectId.GenerateNewId(),
                player.Id,
                question.Id,
                ans.Answer,
                ans.SubmittedAt));
        }

        var mapping = new RoundMapping(
            ObjectId.GenerateNewId(),
            question.Id,
            drawerPlayer.Id,
            correctMappings,
            incorrectMappings,
            round.DrawingBoard?.Schema ?? "{}",
            DateTimeOffset.UtcNow);

        await db.RoundCollection.InsertOneAsync(mapping);
    }

    public async Task<IEnumerable<Round>?> GetAllAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var mappings = await db.RoundCollection
            .Find(Builders<RoundMapping>.Filter.Empty)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        if (mappings == null || mappings.Count == 0)
        {
            return null;
        }

        return await MapToDomainAsync(mappings);
    }

    public async Task<IEnumerable<Round>?> GetAllAsync(string question)
    {
        var questionMapping = await db.QuestionCollection
            .Find(q => q.Content == question)
            .FirstOrDefaultAsync();

        if (questionMapping is null)
        {
            return null;
        }

        var filter = Builders<RoundMapping>.Filter.Eq(r => r.QuestionId, questionMapping.Id);
        var mappings = await db.RoundCollection.Find(filter).ToListAsync();

        if (mappings == null || mappings.Count == 0)
        {
            return null;
        }

        return await MapToDomainAsync(mappings);
    }

    public async Task<IEnumerable<Round>?> GetAllAsync(BraceletId drawerId)
    {
        var player = await db.PlayerCollection
            .Find(p => p.BraceletId == drawerId.Value)
            .FirstOrDefaultAsync();

        if (player is null)
        {
            return null;
        }

        var filter = Builders<RoundMapping>.Filter.Eq(r => r.DrawerId, player.Id);
        var mappings = await db.RoundCollection.Find(filter).ToListAsync();

        if (mappings == null || mappings.Count == 0)
        {
            return null;
        }

        return await MapToDomainAsync(mappings);
    }

    private async Task<List<Round>> MapToDomainAsync(IEnumerable<RoundMapping> mappings)
    {
        var rounds = new List<Round>();

        foreach (var mapping in mappings)
        {
            var questionMapping = await db.QuestionCollection
                .Find(q => q.Id == mapping.QuestionId)
                .FirstOrDefaultAsync();

            var playerMapping = await db.PlayerCollection
                .Find(p => p.Id == mapping.DrawerId)
                .FirstOrDefaultAsync();

            if (questionMapping is null)
            {
                throw new InvalidOperationException($"Question '{mapping.QuestionId}' not found");
            }

            if (playerMapping is null)
            {
                throw new InvalidOperationException($"Player '{mapping.DrawerId}' not found");
            }

            var question = new Question(
                questionMapping.Content,
                questionMapping.Hints.Select(h => new Hint(h)));

            if (questionMapping.UsedAt.HasValue)
            {
                var useStateProperty = typeof(Question).GetProperty(nameof(Question.UseState));
                useStateProperty?.SetValue(question, QuestionUseState.Used);
            }

            var round = new Round(question, new BraceletId(playerMapping.BraceletId));

            // restore drawing board
            round.DrawingBoard = new DrawingBoard(mapping.DrawingBoardSchema ?? "{}");

            // restore answers
            foreach (var ans in mapping.CorrectAnswers ?? new List<AnswerRecordMapping>())
            {
                var ansPlayer = await db.PlayerCollection.Find(p => p.Id == ans.PlayerId).FirstOrDefaultAsync();
                if (ansPlayer is null)
                {
                    throw new InvalidOperationException($"Player '{ans.PlayerId}' not found");
                }

                var record = new AnswerRecord(new BraceletId(ansPlayer.BraceletId), ans.Answer, ans.SubmittedAt);
                round.AddCorrectAnswer(record);
            }

            foreach (var ans in mapping.IncorrectAnswers ?? new List<AnswerRecordMapping>())
            {
                var ansPlayer = await db.PlayerCollection.Find(p => p.Id == ans.PlayerId).FirstOrDefaultAsync();
                if (ansPlayer is null)
                {
                    throw new InvalidOperationException($"Player '{ans.PlayerId}' not found");
                }

                var record = new AnswerRecord(new BraceletId(ansPlayer.BraceletId), ans.Answer, ans.SubmittedAt);
                round.AddIncorrectAnswer(record);
            }

            rounds.Add(round);
        }

        return rounds;
    }
}