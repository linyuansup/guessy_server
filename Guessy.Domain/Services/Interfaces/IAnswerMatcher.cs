using Guessy.Domain.Enums;
using Guessy.Domain.Models;

namespace Guessy.Domain.Services.Interfaces;

public interface IAnswerMatcher
{
    AnswerMatchResult Match(string input, Question question);
}