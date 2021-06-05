using System;
using System.Collections.Generic;
using System.Linq;

namespace RadLine
{
    internal sealed class LineEditorHistory : ILineEditorHistory
    {
        private readonly LinkedList<LineBuffer[]> _history;
        private LinkedListNode<LineBuffer[]>? _current;
        private LineBuffer[]? _intermediate;
        private bool _showIntermediate;

        public LineBuffer[]? Current =>
            _showIntermediate && _intermediate != null
                ? _intermediate : _current?.Value;

        public LineEditorHistory()
        {
            _history = new LinkedList<LineBuffer[]>();
        }

        public void Add(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var lines = text.NormalizeNewLines().Split(new[] { '\n' });
            var buffers = new LineBuffer[lines.Length];
            for (var index = 0; index < lines.Length; index++)
            {
                buffers[index] = new LineBuffer(lines[index]);
            }

            _history.AddLast(buffers);
        }

        internal void Reset()
        {
            _current = null;
            _intermediate = null;
        }

        internal void Add(IList<LineBuffer> buffers)
        {
            if (buffers is null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            _history.AddLast(buffers as LineBuffer[] ?? buffers.ToArray());
            _current = null;
        }

        internal bool MovePrevious(LineEditorState state)
        {
            // At the last one?
            if ((_current == null && _intermediate == null) || _showIntermediate)
            {
                // Got something written that we don't want to lose?
                if (!string.IsNullOrWhiteSpace(state.Text))
                {
                    // Store the interediate buffer so it wont get lost.
                    _intermediate = state.GetBuffers().ToArray();
                }
            }

            _showIntermediate = false;

            if (_current == null && _history.Count > 0)
            {
                _current = _history.Last;
                return true;
            }

            if (_current?.Previous != null)
            {
                _current = _current.Previous;
                return true;
            }

            return false;
        }

        internal bool MoveNext()
        {
            if (_current == null)
            {
                return false;
            }

            if (_current?.Next != null)
            {
                _current = _current.Next;
                return true;
            }

            // Got an intermediate buffer to show?
            if (_intermediate != null)
            {
                _showIntermediate = true;
                return true;
            }

            return false;
        }
    }
}
