using Guessy.Domain.ValueObjects.Ids;
using MediatR;

namespace Guessy.Domain.Events.Answer;

public record AnswerSubmittedEvent(BraceletId PlayerId, string Answer) : INotification;