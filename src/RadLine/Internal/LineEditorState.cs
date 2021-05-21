using System;
using System.Collections.Generic;
using System.Linq;

namespace RadLine
{
    internal sealed class LineEditorState
    {
        private readonly List<LineBuffer> _lines;
        private readonly ILineEditorPrompt _prompt;
        private int _lineIndex;

        public int LineIndex => _lineIndex;
        public bool IsFirstLine => _lineIndex == 0;
        public bool IsLastLine => _lineIndex == _lines.Count - 1;
        public ILineEditorPrompt Prompt => _prompt;
        public LineBuffer Buffer => _lines[_lineIndex];

        public string Text => string.Join(Environment.NewLine, _lines.Select(x => x.Content));

        public LineEditorState(ILineEditorPrompt prompt, string text)
        {
            _lines = new List<LineBuffer>(new[] { new LineBuffer(text) });
            _prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            _lineIndex = 0;
        }

        public bool MoveUp()
        {
            if (_lineIndex > 0)
            {
                _lineIndex--;
                return true;
            }

            return false;
        }

        public bool MoveDown()
        {
            if (_lineIndex < _lines.Count - 1)
            {
                _lineIndex++;
                return true;
            }

            return false;
        }

        public void AddLine()
        {
            _lines.Add(new LineBuffer());
            _lineIndex++;
        }
    }
}
