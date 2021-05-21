using System;
using Spectre.Console;

namespace RadLine
{
    internal sealed class FallbackRenderingStrategy : LineRenderingStrategy
    {
        private readonly IAnsiConsole _console;

        public FallbackRenderingStrategy(IAnsiConsole console, IHighlighterAccessor accessor)
            : base(accessor)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public override void Render(LineEditorState state, int? cursorPosition)
        {
            var (prompt, margin) = state.Prompt.GetPrompt(state.LineIndex);

            // Hide the cursor
            _console.Cursor.Hide();

            // Clear the current line
            Console.CursorLeft = 0;
            Console.Write(new string(' ', _console.Profile.Width));
            Console.CursorLeft = 0;

            // Render the prompt
            _console.Write(prompt);
            _console.Write(new string(' ', margin));

            // Build the buffer
            var width = _console.Profile.Width - prompt.Length - margin - 1;
            var (content, position) = BuildLine(state.Buffer, width, cursorPosition ?? state.Buffer.CursorPosition);

            // Write the buffer
            _console.Write(Highlight(content));

            // Move the cursor to the right position
            Console.CursorLeft = (position ?? 0) + prompt.Length + margin;

            // Show the cursor
            _console.Cursor.Show();
        }
    }
}
