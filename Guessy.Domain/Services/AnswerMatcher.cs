using Guessy.Domain.Enums;
using Guessy.Domain.Models;
using Guessy.Domain.Services.Interfaces;
using System.Text;

namespace Guessy.Domain.Services;

public class AnswerMatcher : IAnswerMatcher
{
    public AnswerMatchResult Match(string input, Question question)
    {
        string normalizedInput = Normalize(input);
        string normalizedAnswer = Normalize(question.Content);
        return normalizedInput == normalizedAnswer ? AnswerMatchResult.Match : AnswerMatchResult.NoMatch;
    }

    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        StringBuilder sb = new(input.Length);
        foreach (char c in from c in input
                 where !char.IsWhiteSpace(c)
                 where !char.IsPunctuation(c)
                 where !char.IsSymbol(c)
                 select c)
        {
            sb.Append(char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }
}