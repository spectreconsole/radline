using System.Collections.Generic;

namespace RadLine
{
    public interface ITextCompletion
    {
        public IEnumerable<string>? GetCompletions(string prefix, string word, string suffix);
    }
}
