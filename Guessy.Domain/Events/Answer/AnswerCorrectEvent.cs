using Guessy.Domain.ValueObjects.Game;
using MediatR;

namespace Guessy.Domain.Events.Answer;

public record AnswerCorrectEvent(AnswerRecord Answer) : INotification;