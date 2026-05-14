using Guessy.Application.Service;
using Guessy.Domain.Events.Answer;
using Guessy.Domain.Repositories;
using Guessy.Domain.ValueObjects.Ids;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Guessy.Presentation.Controllers;

[ApiController]
[Route("api")]
public class PlayerController(IMediator mediator, PlayerService playerService, IPlayerRepository playerRepository)
    : ControllerBase
{
    [HttpGet("uploadAnswer")]
    public async Task<IActionResult> UploadAnswer(string bracelet, string deviceId, string answer)
    {
        var player = await playerRepository.GetAsync(new BraceletId(bracelet));
        if (player == null || player.DeviceId.Value != deviceId)
        {
            return BadRequest();
        }

        await mediator.Publish(new AnswerSubmittedEvent(new(bracelet), answer));
        return Ok();
    }

    [HttpGet("registry")]
    public async Task<IActionResult> Registry(string bracelet, string deviceId)
    {
        var result = await playerService.Register(bracelet, deviceId);
        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet("score")]
    public async Task<IActionResult> GetScore(string bracelet, string deviceId)
    {
        var player = await playerRepository.GetAsync(new BraceletId(bracelet));
        if (player == null || player.DeviceId.Value != deviceId)
        {
            return BadRequest();
        }

        return Ok(player.Score.Value);
    }
}