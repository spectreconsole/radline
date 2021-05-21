using System;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace RadLine
{
    internal abstract class LineRenderingStrategy
    {
        private readonly IHighlighterAccessor _accessor;

        protected LineRenderingStrategy(IHighlighterAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public abstract void Render(LineEditorState state, int? cursorPosition);

        protected (string Content, int? Cursor) BuildLine(LineBuffer buffer, int width, int position)
        {
            var middleOfList = width / 2;

            var skip = 0;
            var take = buffer.Content.Length;
            var pointer = position;

            var scrollable = buffer.Content.Length > width;
            if (scrollable)
            {
                skip = Math.Max(0, position - middleOfList);
                take = Math.Min(width, buffer.Content.Length - skip);

                if (buffer.Content.Length - position < middleOfList)
                {
                    // Pointer should be below the end of the list
                    var diff = middleOfList - (buffer.Content.Length - position);
                    skip -= diff;
                    take += diff;
                    pointer = middleOfList + diff;
                }
                else
                {
                    // Take skip into account
                    pointer -= skip;
                }
            }

            return (
                string.Concat(buffer.Content.Skip(skip).Take(take)),
                pointer);
        }

        protected IRenderable Highlight(string text)
        {
            var highlighter = _accessor.Highlighter;
            if (highlighter == null)
            {
                return new Text(text);
            }

            var paragraph = new Paragraph();
            foreach (var token in StringTokenizer.Tokenize(text))
            {
                var style = string.IsNullOrWhiteSpace(token) ? null : highlighter.Highlight(token);
                paragraph.Append(token, style);
            }

            return paragraph;
        }
    }
}
