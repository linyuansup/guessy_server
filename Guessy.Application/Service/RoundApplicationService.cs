using Guessy.Domain.Enums;
using Guessy.Domain.Events.Answer;
using Guessy.Domain.Events.Round;
using Guessy.Domain.Models;
using Guessy.Domain.Repositories;
using Guessy.Domain.Services.Interfaces;
using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;
using MediatR;

namespace Guessy.Application.Service;

public class RoundApplicationService(
    IAnswerMatcher answerMatcher,
    IScoreCalculator scoreCalculator,
    IMediator mediator,
    IRoundArchiveRepository roundArchiveRepository)
{
    private Round? _round;

    public void SetStroke(string stroke)
    {
        _round?.DrawingBoard = new(stroke);
    }

    public IReadOnlyList<AnswerRecord> CorrectAnswers()
    {
        return _round == null
            ? throw new InvalidOperationException("No round is currently active.")
            : _round.CorrectAnswers;
    }

    public void SubmitAnswer(string answer, BraceletId player)
    {
        if (_round == null)
        {
            throw new InvalidOperationException("No round is currently active.");
        }

        var record = new AnswerRecord(player, answer, DateTime.Now);
        switch (answerMatcher.Match(answer, _round.Question))
        {
            case AnswerMatchResult.Match:
                mediator.Publish(new AnswerCorrectEvent(record));
                break;
            case AnswerMatchResult.NoMatch:
                mediator.Publish(new AnswerWrongEvent(record));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(answer), "Unexpected answer match result.");
        }
    }

    public void AddCorrectAnswer(AnswerRecord record)
    {
        if (_round == null)
        {
            throw new InvalidOperationException("No round is currently active.");
        }

        _round.AddCorrectAnswer(record);
    }

    public void AddWrongAnswer(AnswerRecord record)
    {
        if (_round == null)
        {
            throw new InvalidOperationException("No round is currently active.");
        }

        _round.AddIncorrectAnswer(record);
    }

    public async Task EndRound()
    {
        if (_round == null)
        {
            throw new InvalidOperationException("No round is currently active.");
        }

        var rank = scoreCalculator.CalculateScore(_round);
        await mediator.Publish(new RoundEndEvent(rank));
        await roundArchiveRepository.UpdateAsync(_round);
        _round = null;
    }

    public void StartRound(Round round)
    {
        _round = round;
    }
}