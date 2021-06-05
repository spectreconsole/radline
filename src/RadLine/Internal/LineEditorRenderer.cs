using System;
using System.Text;
using Spectre.Console;
using Spectre.Console.Advanced;

namespace RadLine
{
    internal sealed class LineEditorRenderer
    {
        private readonly IAnsiConsole _console;

        internal LineEditorAnsiBuilder AnsiBuilder { get; }

        public LineEditorRenderer(IAnsiConsole console, IHighlighterAccessor accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _console = console ?? throw new ArgumentNullException(nameof(console));

            AnsiBuilder = new LineEditorAnsiBuilder(_console, accessor);

            if (!_console.Profile.Capabilities.Ansi)
            {
                throw new NotSupportedException("Terminal does not support ANSI");
            }
        }

        public void Initialize(LineEditorState state)
        {
            // Everything fit inside the terminal?
            if (state.LineCount < _console.Profile.Height)
            {
                _console.Cursor.Hide();

                var builder = new StringBuilder();
                for (var lineIndex = 0; lineIndex < state.LineCount; lineIndex++)
                {
                    AnsiBuilder.BuildLine(builder, state, state.GetBufferAt(lineIndex), lineIndex, 0);

                    if (lineIndex != state.LineCount - 1)
                    {
                        builder.Append(Environment.NewLine);
                    }
                }

                _console.WriteAnsi(builder.ToString());
                _console.Cursor.Show();

                // Move to the last line and refresh it
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
            var builder = new StringBuilder();
            AnsiBuilder.BuildRefresh(builder, state);
            _console.WriteAnsi(builder.ToString());
        }

        public void RenderLine(LineEditorState state, int? cursorPosition = null)
        {
            var builder = new StringBuilder();
            AnsiBuilder.BuildLine(builder, state, state.Buffer, null, cursorPosition);
            _console.WriteAnsi(builder.ToString());
        }
    }
}
