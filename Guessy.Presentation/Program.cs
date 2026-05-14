using Guessy.Application;
using Guessy.Application.Service;
using Guessy.Domain.Repositories;
using Guessy.Domain.Services;
using Guessy.Domain.Services.Interfaces;
using Guessy.Infrastructure.Configuration;
using Guessy.Infrastructure.Persistence.DbContext;
using Guessy.Infrastructure.Persistence.Repositories;
using System.Net.WebSockets;
using WebSocketManager = Guessy.Infrastructure.WebSocket.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<PlayerService>();
builder.Services.AddSingleton<RoundApplicationService>();
builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();
builder.Services.AddSingleton<IQuestionRepository, QuestionRepository>();
builder.Services.AddSingleton<IRoundArchiveRepository, RoundArchiveRepository>();
builder.Services.AddSingleton<IAnswerMatcher, AnswerMatcher>();
builder.Services.AddSingleton<IGameConfigService, GameConfigService>();
builder.Services.AddSingleton<IScoreCalculator, ScoreCalculator>();
builder.Services.AddSingleton<WebSocketManager>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<IHostedService>(sp =>
    sp.GetRequiredService<MongoContext>());
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.Configure<DatabaseOption>(builder.Configuration.GetSection("Database"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin() // 允许所有域
            .AllowAnyMethod() // 允许所有 HTTP 方法
            .AllowAnyHeader(); // 允许所有 Header
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseWebSockets();

app.Map("/ws/draw", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketManager = context.RequestServices.GetRequiredService<WebSocketManager>();
        webSocketManager.DrawerWebsocket = new(socket);
        var buffer = new byte[1];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                await socket.ReceiveAsync(
                    buffer,
                    context.RequestAborted);
            }
        }
        finally
        {
            try
            {
                webSocketManager.LeaderboardWebsocket?.Dispose();
            }
            finally
            {
                webSocketManager.LeaderboardWebsocket = null;
            }
        }
    }
});

app.Map("/ws/leaderboard", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketManager = context.RequestServices.GetRequiredService<WebSocketManager>();
        webSocketManager.LeaderboardWebsocket = new(socket);
        var buffer = new byte[1];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                await socket.ReceiveAsync(
                    buffer,
                    context.RequestAborted);
            }
        }
        finally
        {
            try
            {
                webSocketManager.LeaderboardWebsocket?.Dispose();
            }
            finally
            {
                webSocketManager.LeaderboardWebsocket = null;
            }
        }
    }
});

app.MapControllers();
app.Run();