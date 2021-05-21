using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace RadLine
{
    public sealed class LineEditor : IHighlighterAccessor
    {
        private readonly IInputSource _source;
        private readonly IServiceProvider? _provider;
        private readonly IAnsiConsole _console;
        private readonly LineBufferRenderer _presenter;

        public KeyBindings KeyBindings { get; }

        public bool MultiLine { get; init; } = false;
        public string Text { get; init; } = string.Empty;

        public ILineEditorPrompt Prompt { get; init; } = new LineEditorPrompt("[yellow]>[/]");
        public ITextCompletion? Completion { get; init; }
        public IHighlighter? Highlighter { get; init; }

        public LineEditor(IAnsiConsole? terminal = null, IInputSource? source = null, IServiceProvider? provider = null)
        {
            _console = terminal ?? AnsiConsole.Console;
            _source = source ?? new DefaultInputSource(_console);
            _provider = provider;
            _presenter = new LineBufferRenderer(_console, this);

            KeyBindings = new KeyBindings();
            KeyBindings.Add(ConsoleKey.Tab, () => new AutoCompleteCommand(AutoComplete.Next));
            KeyBindings.Add(ConsoleKey.Tab, ConsoleModifiers.Control, () => new AutoCompleteCommand(AutoComplete.Previous));
            KeyBindings.Add<BackspaceCommand>(ConsoleKey.Backspace);
            KeyBindings.Add<DeleteCommand>(ConsoleKey.Delete);
            KeyBindings.Add<MoveHomeCommand>(ConsoleKey.Home);
            KeyBindings.Add<MoveEndCommand>(ConsoleKey.End);
            KeyBindings.Add<MoveUpCommand>(ConsoleKey.UpArrow);
            KeyBindings.Add<MoveDownCommand>(ConsoleKey.DownArrow);
            KeyBindings.Add<MoveFirstLineCommand>(ConsoleKey.PageUp);
            KeyBindings.Add<MoveLastLineCommand>(ConsoleKey.PageDown);
            KeyBindings.Add<MoveLeftCommand>(ConsoleKey.LeftArrow);
            KeyBindings.Add<MoveRightCommand>(ConsoleKey.RightArrow);
            KeyBindings.Add<PreviousWordCommand>(ConsoleKey.LeftArrow, ConsoleModifiers.Control);
            KeyBindings.Add<NextWordCommand>(ConsoleKey.RightArrow, ConsoleModifiers.Control);
            KeyBindings.Add<SubmitCommand>(ConsoleKey.Enter);
            KeyBindings.Add<NewLineCommand>(ConsoleKey.Enter, ConsoleModifiers.Shift);
        }

        public async Task<string?> ReadLine(CancellationToken cancellationToken)
        {
            var cancelled = false;
            var state = new LineEditorState(Prompt, Text);

            while (true)
            {
                var result = await ReadLine(state, cancellationToken).ConfigureAwait(false);

                if (result.Result == SubmitAction.Cancel)
                {
                    cancelled = true;
                    break;
                }
                else if (result.Result == SubmitAction.Submit)
                {
                    break;
                }
                else if (result.Result == SubmitAction.NewLine && MultiLine && state.IsLastLine)
                {
                    AddNewLine(state);
                }
                else if (result.Result == SubmitAction.MoveUp && MultiLine)
                {
                    MoveUp(state);
                }
                else if (result.Result == SubmitAction.MoveDown && MultiLine)
                {
                    MoveDown(state);
                }
                else if (result.Result == SubmitAction.MoveFirst && MultiLine)
                {
                    MoveFirst(state);
                }
                else if (result.Result == SubmitAction.MoveLast && MultiLine)
                {
                    MoveLast(state);
                }
            }

            _presenter.RenderLine(state, cursorPosition: 0);

            // Move the cursor to the last line
            while (state.MoveDown())
            {
                _console.Cursor.MoveDown();
            }

            // Moving the cursor won't work here if we're at
            // the bottom of the screen, so let's insert a new line.
            _console.WriteLine();

            // Return all the lines
            return cancelled ? null : state.Text.TrimEnd('\r', '\n');
        }

        private async Task<(LineBuffer Buffer, SubmitAction Result)> ReadLine(
            LineEditorState state,
            CancellationToken cancellationToken)
        {
            var provider = new DefaultServiceProvider(_provider);
            provider.RegisterOptional<ITextCompletion, ITextCompletion>(Completion);
            var context = new LineEditorContext(state.Buffer, provider);

            _presenter.RenderLine(state);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return (state.Buffer, SubmitAction.Cancel);
                }

                // Get command
                var command = default(LineEditorCommand);
                var key = await _source.ReadKey(cancellationToken).ConfigureAwait(false);
                if (key != null)
                {
                    if (key.Value.KeyChar != 0 && !char.IsControl(key.Value.KeyChar))
                    {
                        command = new InsertCommand(key.Value.KeyChar);
                    }
                    else
                    {
                        command = KeyBindings.GetCommand(key.Value.Key, key.Value.Modifiers);
                    }
                }

                // Execute command
                if (command != null)
                {
                    context.Execute(command);
                }

                // Time to exit?
                if (context.Result != null)
                {
                    return (state.Buffer, context.Result.Value);
                }

                // Render the line
                _presenter.RenderLine(state);
            }
        }

        private void AddNewLine(LineEditorState state)
        {
            using (_console.HideCursor())
            {
                _presenter.RenderLine(state, cursorPosition: 0);
                state.AddLine();

                // Moving the cursor won't work here if we're at
                // the bottom of the screen, so let's insert a new line.
                _console.WriteLine();
            }
        }

        private void MoveUp(LineEditorState state)
        {
            Move(state, state =>
            {
                if (state.MoveUp())
                {
                    _console.Cursor.MoveUp();
                }
            });
        }

        private void MoveDown(LineEditorState state)
        {
            Move(state, state =>
            {
                if (state.MoveDown())
                {
                    _console.Cursor.MoveDown();
                }
            });
        }

        private void MoveFirst(LineEditorState state)
        {
            Move(state, state =>
            {
                while (state.MoveUp())
                {
                    _console.Cursor.MoveUp();
                }
            });
        }

        private void MoveLast(LineEditorState state)
        {
            Move(state, state =>
            {
                while (state.MoveDown())
                {
                    _console.Cursor.MoveDown();
                }
            });
        }

        private void Move(LineEditorState state, Action<LineEditorState> action)
        {
            using (_console.HideCursor())
            {
                _presenter.RenderLine(state, cursorPosition: 0);
                var position = state.Buffer.Position;
                action(state);
                state.Buffer.Move(position);
            }
        }
    }
}
