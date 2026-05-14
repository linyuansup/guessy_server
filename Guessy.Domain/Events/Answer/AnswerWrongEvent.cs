using Guessy.Domain.ValueObjects.Game;
using MediatR;

namespace Guessy.Domain.Events.Answer;

public record AnswerWrongEvent(AnswerRecord Answer) : INotification;