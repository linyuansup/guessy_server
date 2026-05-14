using Guessy.Infrastructure.Configuration;
using Guessy.Infrastructure.Persistence.Index;
using Guessy.Infrastructure.Persistence.Mapping;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Guessy.Infrastructure.Persistence.DbContext;

public class MongoContext(IOptions<DatabaseOption> databaseOption) : IHostedService
{
    private readonly MongoClient _client = new(databaseOption.Value.ConnectionString);
    private readonly string _databaseName = databaseOption.Value.DatabaseName;
    public IMongoCollection<QuestionMapping> QuestionCollection { get; private set; } = null!;
    public IMongoCollection<PlayerMapping> PlayerCollection { get; private set; } = null!;
    public IMongoCollection<RoundMapping> RoundCollection { get; private set; } = null!;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var database = _client.GetDatabase(_databaseName);
        PlayerCollection = database.GetCollection<PlayerMapping>("Players");
        QuestionCollection = database.GetCollection<QuestionMapping>("Questions");
        RoundCollection = database.GetCollection<RoundMapping>("Rounds");
        await PlayerIndex.CreateAsync(PlayerCollection, cancellationToken);
        await QuestionIndex.CreateOneAsync(QuestionCollection, cancellationToken);
        await RoundIndex.CreateAsync(RoundCollection, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}