using System;
using System.Globalization;
using System.Linq;

namespace RadLine
{
    public sealed class LineBuffer
    {
        private string _initialContent;
        private string _buffer;
        private int _position;

        public int Position => _position;
        public int Length => _buffer.Length;
        public string InitialContent => _initialContent;
        public string Content => _buffer;

        public bool AtBeginning => Position == 0;
        public bool AtEnd => Position == Content.Length;

        public bool IsAtCharacter
        {
            get
            {
                if (Length == 0)
                {
                    return false;
                }

                if (AtEnd)
                {
                    return false;
                }

                return !char.IsWhiteSpace(_buffer[_position]);
            }
        }

        public bool IsAtBeginningOfWord
        {
            get
            {
                if (Length == 0)
                {
                    return false;
                }

                if (_position == 0)
                {
                    return !char.IsWhiteSpace(_buffer[0]);
                }

                return char.IsWhiteSpace(_buffer[_position - 1]);
            }
        }

        public bool IsAtEndOfWord
        {
            get
            {
                if (Length == 0)
                {
                    return false;
                }

                if (_position == 0)
                {
                    return false;
                }

                return !char.IsWhiteSpace(_buffer[_position - 1]);
            }
        }

        // TODO: Right now, this only returns the position in the line buffer.
        // This is OK for western alphabets and most emojis which consist
        // of a single surrogate pair, but everything else will be wrong.
        public int CursorPosition => _position;

        public LineBuffer(string? content = null)
        {
            _initialContent = content ?? string.Empty;
            _buffer = _initialContent;
            _position = _buffer.Length;
        }

        public LineBuffer(LineBuffer buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _initialContent = buffer.InitialContent;
            _buffer = buffer.Content;
            _position = _buffer.Length;
        }

        public bool Move(int position)
        {
            if (position == _position)
            {
                return false;
            }

            var movingLeft = position < _position;
            _position = MoveToPosition(position, movingLeft);

            return true;
        }

        public void Insert(char character)
        {
            _buffer = _buffer.Insert(_position, character.ToString());
        }

        public void Insert(string text)
        {
            _buffer = _buffer.Insert(_position, text);
        }

        public void Reset()
        {
            _buffer = _initialContent;
            _position = _buffer.Length;
        }

        public int Clear(int index, int count)
        {
            if (index < 0)
            {
                return 0;
            }

            if (index > _buffer.Length - 1)
            {
                return 0;
            }

            var length = _buffer.Length;
            _buffer = _buffer.Remove(Math.Max(0, index), Math.Min(count, _buffer.Length - index));
            return Math.Max(length - _buffer.Length, 0);
        }

        private int MoveToPosition(int position, bool movingLeft)
        {
            if (position <= 0)
            {
                return 0;
            }
            else if (position >= _buffer.Length)
            {
                return _buffer.Length;
            }

            var indices = StringInfo.ParseCombiningCharacters(_buffer);

            if (movingLeft)
            {
                foreach (var e in indices.Reverse())
                {
                    if (e <= position)
                    {
                        return e;
                    }
                }
            }
            else
            {
                foreach (var e in indices)
                {
                    if (e >= position)
                    {
                        return e;
                    }
                }
            }

            throw new InvalidOperationException("Could not find position in buffer");
        }
    }
}
