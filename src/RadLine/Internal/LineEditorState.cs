using System;
using System.Collections.Generic;
using System.Linq;

namespace RadLine
{
    internal sealed class LineEditorState : ILineEditorState
    {
        private readonly List<LineBuffer> _lines;
        private int _lineIndex;

        public ILineEditorPrompt Prompt { get; }

        public int LineIndex => _lineIndex;
        public int LineCount => _lines.Count;
        public bool IsFirstLine => _lineIndex == 0;
        public bool IsLastLine => _lineIndex == _lines.Count - 1;
        public LineBuffer Buffer => _lines[_lineIndex];

        public string Text => string.Join(Environment.NewLine, _lines.Select(x => x.Content));

        public LineEditorState(ILineEditorPrompt prompt, string text)
        {
            _lines = new List<LineBuffer>();
            _lineIndex = 0;

            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));

            // Add all lines
            foreach (var line in text.NormalizeNewLines().Split(new[] { '\n' }))
            {
                _lines.Add(new LineBuffer(line));
            }

            // No lines?
            if (_lines.Count == 0)
            {
                _lines.Add(new LineBuffer());
            }
        }

        public LineBuffer GetBufferAt(int line)
        {
            return _lines[line];
        }

        public void Move(int line)
        {
            _lineIndex = Math.Max(0, Math.Min(line, LineCount - 1));
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
