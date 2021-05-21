using System;
using System.Collections.Generic;
using Spectre.Console;

namespace RadLine
{
    public sealed class WordHighlighter : IHighlighter
    {
        private readonly Dictionary<string, Style> _words;

        public WordHighlighter(StringComparer? comparer = null)
        {
            _words = new Dictionary<string, Style>(comparer ?? StringComparer.OrdinalIgnoreCase);
        }

        public WordHighlighter AddWord(string word, Style style)
        {
            _words[word] = style;
            return this;
        }

        Style? IHighlighter.Highlight(string token)
        {
            _words.TryGetValue(token, out var style);
            return style;
        }
    }
}
