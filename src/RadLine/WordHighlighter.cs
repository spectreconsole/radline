using System;
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;
using static System.Net.Mime.MediaTypeNames;

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

        IRenderable IHighlighter.BuildHighlightedText(string text)
        {
            var paragraph = new Paragraph();
            foreach (var token in StringTokenizer.Tokenize(text))
            {
                if (_words.TryGetValue(token, out var style))
                {
                    paragraph.Append(token, style);
                }
                else
                {
                    paragraph.Append(token, null);
                }
            }

            return paragraph;
        }
    }
}
