using System;
using System.Linq;
using System.Text;
using Spectre.Console;
using Spectre.Console.Advanced;
using Spectre.Console.Rendering;

namespace RadLine
{
    internal sealed class LineEditorAnsiBuilder
    {
        private readonly IAnsiConsole _console;
        private readonly IHighlighterAccessor _accessor;

        public LineEditorAnsiBuilder(IAnsiConsole console, IHighlighterAccessor accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _console = console ?? throw new ArgumentNullException(nameof(console));
            _accessor = accessor;
        }

        public void BuildClear(StringBuilder builder, LineEditorState state)
        {
            if (state.LineIndex != 0)
            {
                // Move the cursor to the top
                builder.Append("\u001b[").Append(state.LineIndex).Append('A');
            }

            // Clear lines
            var emptyLine = new string(' ', _console.Profile.Width);
            for (var i = 0; i < state.LineCount; i++)
            {
                builder.Append("\u001b[2K");

                if (i != state.LineCount - 1)
                {
                    // Move down
                    builder.Append("\u001b[1B");
                }
            }

            // Move back to the top
            var position = state.LineCount - 1;
            if (position > 0)
            {
                builder.Append("\u001b[").Append(position).Append('A');
            }

            // Set cursor to beginning of line
            builder.Append("\u001b[1G");
        }

        public void BuildRefresh(StringBuilder builder, LineEditorState state)
        {
            if (state.LineCount > _console.Profile.Height)
            {
                BuildFullDisplayRefresh(builder, state);
            }
            else
            {
                BuildPartialDisplayRefresh(builder, state);
            }
        }

        public int? BuildLine(StringBuilder builder, LineEditorState state, LineBuffer buffer, int? lineIndex = null, int? cursorPosition = null)
        {
            builder.Append("\u001b[2K"); // Clear the current line
            builder.Append("\u001b[1G"); // Set cursor to beginning of line

            var (prompt, margin) = state.Prompt.GetPrompt(state, lineIndex ?? state.LineIndex);

            cursorPosition ??= state.Buffer.CursorPosition;

            // Render the prompt
            builder.Append(_console.ToAnsi(prompt));
            builder.Append(new string(' ', margin));

            // Build the buffer
            var width = _console.Profile.Width - prompt.Length - margin - 1;
            var (line, position) = BuildLineContent(buffer, width, cursorPosition.Value);

            // Build the resulting ANSI
            var ansi = _console.ToAnsi(BuildHighlightedText(line));

            // Need to pad the content?
            if (line.Length < width)
            {
                ansi = ansi.PadRight(width);
            }

            // Output the buffer
            builder.Append(ansi);

            // Move the cursor to the right position
            var cursorPos = position + prompt.Length + margin + 1;
            builder.Append("\u001b[").Append(cursorPos).Append('G');

            // Return the position
            return position;
        }

        public void MoveDown(StringBuilder builder, LineEditorState state)
        {
            if (state.LineCount > _console.Profile.Height)
            {
                builder.Append("\u001b[1B");
            }
            else
            {
                builder.AppendLine();
            }
        }

        private void BuildFullDisplayRefresh(StringBuilder builder, LineEditorState state)
        {
            builder.Append("\u001b[1;1H"); // Set cursor to home position

            var lineIndex = 0;
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

                lineIndex = skip;
            }

            // Render all lines
            for (var i = 0; i < Math.Min(state.LineCount, height); i++)
            {
                // Set cursor to beginning of line
                builder.Append("\u001b[1G");

                // Render the line
                BuildLine(builder, state, state.GetBufferAt(lineIndex), lineIndex, 0);

                // Move cursor down
                builder.Append("\u001b[1E");
                lineIndex++;
            }

            // Position the cursor at the right line
            builder.Append("\u001b[").Append(pointer + 1).Append(";1H");

            // Refresh the current line
            BuildLine(builder, state, state.Buffer, null);
        }

        private void BuildPartialDisplayRefresh(StringBuilder builder, LineEditorState state)
        {
            if (state.LineIndex > 0)
            {
                // Move the cursor up
                builder.Append("\u001b[").Append(state.LineIndex).Append('A');
            }

            // Render all lines
            for (var lineIndex = 0; lineIndex < state.LineCount; lineIndex++)
            {
                // Set cursor to beginning of line
                builder.Append("\u001b[1G");

                // Render the line
                BuildLine(builder, state, state.GetBufferAt(lineIndex), lineIndex, 0);
                builder.Append('\n');
            }

            // Position the cursor at the right line
            var moveUp = state.LineCount - state.LineIndex;
            builder.Append("\u001b[").Append(moveUp).Append('A');

            // Refresh the current line
            BuildLine(builder, state, state.Buffer, null);
        }

        private static (string Content, int? Cursor) BuildLineContent(LineBuffer buffer, int width, int position)
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

        private IRenderable BuildHighlightedText(string text)
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
    }
}
