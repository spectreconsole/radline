using System;
using Spectre.Console;

namespace RadLine
{
    public sealed class LineEditorPrompt : ILineEditorPrompt
    {
        private readonly Markup _prompt;
        private readonly Markup? _more;

        public LineEditorPrompt(string prompt, string? more = null)
        {
            if (prompt is null)
            {
                throw new ArgumentNullException(nameof(prompt));
            }

            _prompt = new Markup(prompt);
            _more = more != null ? new Markup(more) : null;

            if (_prompt.Lines > 1)
            {
                throw new ArgumentException("Prompt cannot contain line breaks", nameof(prompt));
            }

            if (_more?.Lines > 1)
            {
                throw new ArgumentException("Prompt cannot contain line breaks", nameof(more));
            }
        }

        public (Markup Markup, int Margin) GetPrompt(ILineEditorState state, int line)
        {
            if (line == 0)
            {
                return (_prompt, 1);
            }

            return (_more ?? _prompt, 1);
        }
    }
}
