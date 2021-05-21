using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RadLine
{
    public interface ITextCompletion
    {
        public IEnumerable<string>? GetCompletions(string prefix, string word, string suffix);
    }

    public static class TextCompletionExtensions
    {
        public static bool TryGetCompletions(
            this ITextCompletion completion,
            string prefix, string word, string suffix,
            [NotNullWhen(true)] out string[]? result)
        {
            var completions = completion.GetCompletions(prefix, word, suffix);
            if (completions == null || !completions.Any())
            {
                result = null;
                return false;
            }

            result = completions.ToArray();
            return true;
        }
    }
}
