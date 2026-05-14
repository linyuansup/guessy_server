using Guessy.Application.Service;
using Guessy.Domain.Models;
using Guessy.Domain.Repositories;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;
using Microsoft.AspNetCore.Mvc;

namespace Guessy.Presentation.Controllers;

[ApiController]
[Route("api")]
public class AdminController(
    RoundApplicationService roundService,
    IQuestionRepository questionRepository,
    IRoundArchiveRepository roundArchiveRepository,
    IPlayerRepository playerRepository)
    : ControllerBase
{
    [HttpGet("stop")]
    public async Task<IActionResult> StopGame()
    {
        await roundService.EndRound();
        return Ok();
    }

    [HttpGet("getProblem")]
    public async Task<IActionResult> GetProblem(int page, int pageSize)
    {
        return Ok(await questionRepository.GetAsync(page, pageSize));
    }

    [HttpPost("updateProblem")]
    public async Task<IActionResult> UpdateProblem(CreateQuestionRequest question)
    {
        var q = new Question(
            question.Content, question.Hints.Select(h => new Hint(h.Value))
        );
        if (question.Used)
        {
            q.UseState = Domain.Enums.QuestionUseState.Used;
        }
        await questionRepository.UpdateAsync(q);
        return Ok();
    }

    [HttpGet("deleteProblem")]
    public async Task<IActionResult> DeleteProblem(string content)
    {
        await questionRepository.DeleteAsync(content);
        return Ok();
    }

    [HttpGet("find")]
    public async Task<IActionResult> Find(string content)
    {
        return Ok(await questionRepository.GetAsync(content));
    }

    [HttpGet("getRoundArchive")]
    public async Task<IActionResult> GetRoundArchive(int page, int pageSize)
    {
        return Ok(await roundArchiveRepository.GetAllAsync(page, pageSize));
    }

    [HttpGet("findRoundArchiveByQuestion")]
    public async Task<IActionResult> FindRoundArchiveByQuestion(string question)
    {
        return Ok(await roundArchiveRepository.GetAllAsync(question));
    }

    [HttpGet("findRoundArchiveByDrawerId")]
    public async Task<IActionResult> FindRoundArchiveByDrawerId(string drawerId)
    {
        return Ok(await roundArchiveRepository.GetAllAsync(new BraceletId(drawerId)));
    }

    [HttpGet("getPlayerByBraceletId")]
    public async Task<IActionResult> GetPlayerByBraceletId(string braceletId)
    {
        return Ok(await playerRepository.GetAsync(new BraceletId(braceletId)));
    }

    [HttpGet("getPlayers")]
    public async Task<IActionResult> GetPlayers(int page, int pageSize)
    {
        return Ok(await playerRepository.GetAllAsync(page, pageSize));
    }

    [HttpGet("deletePlayer")]
    public async Task<IActionResult> DeletePlayer(string braceletId)
    {
        await playerRepository.DeleteAsync(new BraceletId(braceletId));
        return Ok();
    }

    public class CreateQuestionRequest
    {
        public string Content { get; set; } = "";

        public List<CreateHintRequest> Hints { get; set; } = [];
        public bool Used { get; set; } = false;
    }

    public class CreateHintRequest
    {
        public string Value { get; set; } = "";
    }
}