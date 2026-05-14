using Guessy.Domain.Events.Leaderboard;
using Guessy.Domain.Events.Round;
using Guessy.Domain.Models;
using Guessy.Domain.Repositories;
using Guessy.Domain.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Guessy.Presentation.Controllers;

[ApiController]
[Route("api")]
public class DrawerController(
    IMediator mediator,
    IQuestionRepository questionRepository,
    IGameConfigService gameConfigService,
    Guessy.Infrastructure.WebSocket.WebSocketManager webSocketManager,
    IPlayerRepository playerRepository) : ControllerBase
{
    public class ConfigResponseDto
    {
        public int DisplayTopCount { get; set; }
        public int Duration { get; set; }
        public int StopNumber { get; set; }
    }

    public class UpdateConfigRequestDto
    {
        public int DisplayTopCount { get; set; }
        public int Duration { get; set; }
        public List<RankRewardRequestDto> RankRewards { get; set; } = [];
        public int SelectQuestionCount { get; set; }
    }

    public class RankRewardRequestDto
    {
        public int Rank { get; set; }
        public int PlayerScore { get; set; }
        public int DrawerScore { get; set; }
    }

    public class DrawRequestDto
    {
        public string Stroke { get; set; } = string.Empty;
    }
    [HttpPost("draw")]
    public IActionResult Draw([FromBody] DrawRequestDto dto)
    {
        string time = DateTime.Now.ToString();
        mediator.Publish(new StrokeChangeEvent(dto.Stroke));
        return Ok();
    }

    [HttpGet("start")]
    public async Task<IActionResult> Start(string braceletId, string question)
    {
        if (webSocketManager.DrawerWebsocket == null || webSocketManager.LeaderboardWebsocket == null)
        {
            return BadRequest();
        }
        var player = await playerRepository.GetAsync(new Domain.ValueObjects.Ids.BraceletId(braceletId));
        if (player == null)
        {
            return BadRequest();
        }

        var problem = await questionRepository.GetAsync(question);
        if (problem == null)
        {
            return BadRequest();
        }

        var round = new Round(problem, new(braceletId));
        await webSocketManager.LeaderboardWebsocket.SendGameStart(problem);
        await mediator.Publish(new RoundStartEvent(round));
        return Ok();
    }

    [HttpGet("unused")]
    public async Task<IActionResult> Unused()
    {
        var question =
            await questionRepository.GetUnusedAsync(gameConfigService.Config.RoundConfig.SelectQuestionCount);
        return Ok(question);
    }

    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        var config = gameConfigService.Config;
        return Ok(new ConfigResponseDto
        {
            DisplayTopCount = config.LeaderboardConfig.DisplayTopCount,
            Duration = (int)config.RoundConfig.Duration.TotalSeconds,
            StopNumber = config.ScoreConfig.RankRewards.Count()
        });
    }

    [HttpPost("config")]
    public IActionResult UpdateConfig([FromBody] UpdateConfigRequestDto dto)
    {
        var scoreConfig = new Guessy.Domain.ValueObjects.Config.ScoreConfig(
            dto.RankRewards.Select(r => new Guessy.Domain.ValueObjects.Config.RankReward(
                r.Rank,
                new Guessy.Domain.ValueObjects.Game.Score(r.PlayerScore),
                new Guessy.Domain.ValueObjects.Game.Score(r.DrawerScore))));
        var roundConfig = new Guessy.Domain.ValueObjects.Config.RoundConfig(
            TimeSpan.FromSeconds(dto.Duration),
            dto.SelectQuestionCount);
        var leaderboardConfig = new Guessy.Domain.ValueObjects.Config.LeaderboardConfig(dto.DisplayTopCount);

        gameConfigService.Update(new Guessy.Domain.Models.GameConfig(scoreConfig, roundConfig, leaderboardConfig));
        return Ok();
    }
}