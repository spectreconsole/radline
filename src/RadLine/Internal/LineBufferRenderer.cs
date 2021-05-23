using System;
using System.Linq;
using System.Text;
using Spectre.Console;
using Spectre.Console.Advanced;
using Spectre.Console.Rendering;

namespace RadLine
{
    internal sealed class LineBufferRenderer
    {
        private readonly IAnsiConsole _console;
        private readonly IHighlighterAccessor _accessor;

        public LineBufferRenderer(IAnsiConsole console, IHighlighterAccessor accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _console = console ?? throw new ArgumentNullException(nameof(console));
            _accessor = accessor;
        }

        public void Initialize(LineEditorState state)
        {
            // Everything fit inside the terminal?
            if (state.LineCount < _console.Profile.Height)
            {
                _console.Cursor.Hide();

                var builder = new StringBuilder();
                for (var i = 0; i < state.LineCount; i++)
                {
                    var (prompt, margin) = state.Prompt.GetPrompt(state, i);
                    AppendLine(builder, state.GetBufferAt(i), prompt, margin, 0);

                    if (i != state.LineCount - 1)
                    {
                        builder.Append(Environment.NewLine);
                    }
                }

                _console.WriteAnsi(builder.ToString());
                _console.Cursor.Show();

                // Move to the last line
                state.Move(state.LineCount);
                RenderLine(state);
            }
            else
            {
                Refresh(state);
            }
        }

        public void Refresh(LineEditorState state)
        {
            if (!_console.Profile.Capabilities.Ansi)
            {
                throw new NotSupportedException("Terminal does not support ANSI");
            }

            var builder = new StringBuilder();
            builder.Append("\u001b[1;1H"); // Set cursor to home position

            var rowIndex = 0;
            var height = _console.Profile.Height;
            var lineCount = state.LineCount - 1;
            var middleOfList = height / 2;
            var offset = (height % 2 == 0) ? 1 : 0;
            var pointer = state.LineIndex;

            // Calculate the visible part
            var scrollable = lineCount >= height;
            if (scrollable)
            {
                var skip = Math.Max(0, state.LineIndex - middleOfList);

                if (lineCount - state.LineIndex < middleOfList)
                {
                    // Pointer should be below the end of the list
                    var diff = middleOfList - (lineCount - state.LineIndex);
                    skip -= diff - offset;
                    pointer = middleOfList + diff - offset;
                }
                else
                {
                    // Take skip into account
                    pointer -= skip;
                }

                rowIndex = skip;
            }

            // Render all lines
            for (var i = 0; i < Math.Min(state.LineCount, height); i++)
            {
                // Set cursor to beginning of line
                builder.Append("\u001b[1G");

                // Render the line
                var (prompt, margin) = state.Prompt.GetPrompt(state, rowIndex);
                AppendLine(builder, state.GetBufferAt(rowIndex), prompt, margin, 0);

                // Move cursor down
                builder.Append("\u001b[1E");
                rowIndex++;
            }

            // Position the cursor at the right line
            builder.Append("\u001b[").Append(pointer + 1).Append(";1H");

            // Flush
            _console.WriteAnsi(builder.ToString());

            // Refresh the current line
            RenderLine(state);
        }

        public void RenderLine(LineEditorState state, int? cursorPosition = null)
        {
            if (!_console.Profile.Capabilities.Ansi)
            {
                throw new NotSupportedException("Terminal does not support ANSI");
            }

            var builder = new StringBuilder();

            // Prepare
            builder.Append("\u001b[?7l"); // Autowrap off
            builder.Append("\u001b[2K"); // Clear the current line
            builder.Append("\u001b[1G"); // Set cursor to beginning of line

            // Append the whole line
            var (prompt, margin) = state.Prompt.GetPrompt(state, state.LineIndex);
            var position = AppendLine(builder, state.Buffer, prompt, margin, cursorPosition ?? state.Buffer.CursorPosition);

            // Move the cursor to the right position
            var cursorPos = position + prompt.Length + margin + 1;
            builder.Append("\u001b[").Append(cursorPos).Append('G');

            // Flush
            _console.WriteAnsi(builder.ToString());

            // Turn on auto wrap
            builder.Append("\u001b[?7h");
        }

        private int? AppendLine(StringBuilder builder, LineBuffer buffer, Markup prompt, int margin, int cursorPosition)
        {
            // Render the prompt
            builder.Append(_console.ToAnsi(prompt));
            builder.Append(new string(' ', margin));

            // Build the buffer
            var width = _console.Profile.Width - prompt.Length - margin - 1;
            var (content, position) = BuildLine(buffer, width, cursorPosition);

            var output = _console.ToAnsi(Highlight(content));
            if (output.Length < width)
            {
                output = output.PadRight(width);
            }

            // Output the buffer
            builder.Append(output);

            // Return the position
            return position;
        }

        private IRenderable Highlight(string text)
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

        private static (string Content, int? Cursor) BuildLine(LineBuffer buffer, int width, int position)
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
    }
}
