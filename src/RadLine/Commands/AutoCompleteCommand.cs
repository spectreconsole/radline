using System;

namespace RadLine
{
    public sealed class AutoCompleteCommand : LineEditorCommand
    {
        private const string Position = nameof(Position);
        private const string Index = nameof(Index);

        private readonly AutoComplete _kind;

        public AutoCompleteCommand(AutoComplete kind)
        {
            _kind = kind;
        }

        public override void Execute(LineEditorContext context)
        {
            var completion = context.GetService<ITextCompletion>();
            if (completion == null)
            {
                return;
            }

            var originalPosition = context.Buffer.Position;

            // Get the start position of the word
            var start = context.Buffer.Position;
            if (context.Buffer.IsAtCharacter)
            {
                context.Buffer.MoveToBeginningOfWord();
                start = context.Buffer.Position;
            }
            else if (context.Buffer.IsAtEndOfWord)
            {
                context.Buffer.MoveToPreviousWord();
                start = context.Buffer.Position;
            }

            // Get the end position of the word.
            var end = context.Buffer.Position;
            if (context.Buffer.IsAtCharacter)
            {
                context.Buffer.MoveToEndOfWord();
                end = context.Buffer.Position;
            }

            // Not the same start position as last time?
            if (context.GetState(Position, () => 0) != start)
            {
                // Reset
                var startIndex = _kind == AutoComplete.Next ? 0 : -1;
                context.SetState(Index, startIndex);
            }

            // Get the prefix and word
            var prefix = context.Buffer.Content.Substring(0, start);
            var word = context.Buffer.Content.Substring(start, end - start);
            var suffix = context.Buffer.Content.Substring(end, context.Buffer.Content.Length - end);

            // Get the completions
            if (!completion.TryGetCompletions(prefix, word, suffix, out var completions))
            {
                context.Buffer.Move(originalPosition);
                return;
            }

            // Get the index to insert
            var index = GetSuggestionIndex(context, word, completions);
            if (index == -1)
            {
                context.Buffer.Move(originalPosition);
                return;
            }

            // Remove the current word
            if (start != end)
            {
                context.Buffer.Clear(start, end - start);
                context.Buffer.Move(start);
            }

            // Insert the completion
            context.Buffer.Insert(completions[index]);

            // Move to the end of the word
            context.Buffer.MoveToEndOfWord();

            // Increase the completion index
            context.SetState(Position, start);
            context.SetState(Index, _kind == AutoComplete.Next ? ++index : --index);
        }

        private int GetSuggestionIndex(LineEditorContext context, string word, string[] completions)
        {
            if (completions is null)
            {
                throw new ArgumentNullException(nameof(completions));
            }

            if (!string.IsNullOrWhiteSpace(word))
            {
                // Try find an exact match
                var index = 0;
                foreach (var completion in completions)
                {
                    if (completion.Equals(word, StringComparison.Ordinal))
                    {
                        var newIndex = _kind == AutoComplete.Next ? index + 1 : index - 1;
                        return newIndex.WrapAround(0, completions.Length - 1);
                    }

                    index++;
                }

                // Try find a partial match
                index = 0;
                foreach (var completion in completions)
                {
                    if (completion.StartsWith(word, StringComparison.Ordinal))
                    {
                        return index;
                    }

                    index++;
                }

                return -1;
            }

            return context.GetState(Index, () => 0).WrapAround(0, completions.Length - 1);
        }
    }
}
