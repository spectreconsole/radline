namespace RadLine
{
    public static class LineBufferExtensions
    {
        public static bool MoveLeft(this LineBuffer buffer, int count = 1)
        {
            return buffer.Move(buffer.Position - count);
        }

        public static bool MoveRight(this LineBuffer buffer, int count = 1)
        {
            return buffer.Move(buffer.Position + count);
        }

        public static bool MoveHome(this LineBuffer buffer)
        {
            return buffer.Move(0);
        }

        public static bool MoveEnd(this LineBuffer buffer)
        {
            return buffer.Move(buffer.Length);
        }

        public static bool MoveToPreviousWord(this LineBuffer buffer)
        {
            var position = buffer.Position;

            if (buffer.IsAtCharacter)
            {
                if (buffer.IsAtBeginningOfWord)
                {
                    // At the beginning of a word.
                    // Move to the left side of the word.
                    buffer.MoveLeft();

                    // Move left until we encounter a new word
                    while (buffer.Position > 0 && !buffer.IsAtCharacter)
                    {
                        buffer.MoveLeft();
                    }
                }
            }
            else
            {
                // Move until we encounter a word
                while (buffer.Position > 0 && !buffer.IsAtCharacter)
                {
                    buffer.MoveLeft();
                }
            }

            // Move to the beginning of the word
            buffer.MoveToBeginningOfWord();

            // Return whether or not we moved
            return position != buffer.Position;
        }

        public static bool MoveToNextWord(this LineBuffer buffer)
        {
            var position = buffer.Position;

            if (buffer.IsAtCharacter)
            {
                // Move to the end of the word
                buffer.MoveToEndOfWord();

                // Move past any space
                while (!buffer.AtEnd && !buffer.IsAtCharacter)
                {
                    buffer.MoveRight();
                }
            }
            else
            {
                // Move past any space
                while (!buffer.AtEnd && !buffer.IsAtCharacter)
                {
                    buffer.MoveRight();
                }
            }

            // Return whether or not we moved
            return position != buffer.Position;
        }

        public static bool MoveToEndOfWord(this LineBuffer buffer)
        {
            if (buffer.AtEnd)
            {
                return false;
            }

            // Not at a word? Do nothing
            if (!buffer.IsAtCharacter)
            {
                return false;
            }

            var position = buffer.Position;

            // Move until we find whitespace
            while (!buffer.AtEnd && buffer.IsAtCharacter)
            {
                buffer.MoveRight();
            }

            // Return whether or not we moved
            return position != buffer.Position;
        }

        public static bool MoveToBeginningOfWord(this LineBuffer buffer)
        {
            if (buffer.Position == 0)
            {
                return false;
            }

            // Not at a word? Do nothing
            if (!buffer.IsAtCharacter)
            {
                return false;
            }

            var position = buffer.Position;

            // Move until previous character is whitespace
            while (buffer.Position > 0)
            {
                if (char.IsWhiteSpace(buffer.Content[buffer.Position - 1]))
                {
                    break;
                }

                buffer.MoveLeft();
            }

            // Return whether or not we moved
            return position != buffer.Position;
        }
    }
}
