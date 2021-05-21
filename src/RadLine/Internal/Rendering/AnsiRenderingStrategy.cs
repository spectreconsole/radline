using System;
using System.Text;
using Spectre.Console;
using Spectre.Console.Advanced;

namespace RadLine
{
    internal sealed class AnsiRenderingStrategy : LineRenderingStrategy
    {
        private readonly IAnsiConsole _console;

        public AnsiRenderingStrategy(IAnsiConsole console, IHighlighterAccessor accessor)
            : base(accessor)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public override void Render(LineEditorState state, int? cursorPosition)
        {
            var builder = new StringBuilder();

            var (prompt, margin) = state.Prompt.GetPrompt(state.LineIndex);

            // Prepare
            builder.Append("\u001b[?7l"); // Autowrap off
            builder.Append("\u001b[2K"); // Clear the current line
            builder.Append("\u001b[1G"); // Set cursor to beginning of line

            // Render the prompt
            builder.Append(_console.ToAnsi(prompt));
            builder.Append(new string(' ', margin));

            // Build the buffer
            var width = _console.Profile.Width - prompt.Length - margin - 1;
            var (content, position) = BuildLine(state.Buffer, width, cursorPosition ?? state.Buffer.CursorPosition);

            // Output the buffer
            builder.Append(_console.ToAnsi(Highlight(content)));

            // Move the cursor to the right position
            var cursorPos = position + prompt.Length + margin + 1;
            builder.Append("\u001b[").Append(cursorPos).Append('G');

            // Flush
            _console.WriteAnsi(builder.ToString());

            // Turn on auto wrap
            builder.Append("\u001b[?7h");
        }
    }
}
