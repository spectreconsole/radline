using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace RadLine.Examples
{
    public static class Program
    {
        public static async Task Main()
        {
            if (!LineEditor.IsSupported(AnsiConsole.Console))
            {
                AnsiConsole.MarkupLine("The terminal does not support ANSI codes, or it isn't a terminal.");
            }

            AnsiConsole.Render(new FigletText("BASIC REPL"));
            
            // Create editor
            var editor = new LineEditor()
            {
                MultiLine = true,
                Text = "PRINT \"Hello\"",
                Prompt = new LineNumberPrompt(new Style(foreground: Color.Yellow)),
                Highlighter = new WordHighlighter()
                    .AddWord("$", new Style(foreground: Color.Yellow))
                    .AddWord("INPUT", new Style(foreground: Color.Blue))
                    .AddWord("PRINT", new Style(foreground: Color.Blue)),
            };

            // Add custom commands
            editor.KeyBindings.Clear();
            editor.KeyBindings.Add<BackspaceCommand>(ConsoleKey.Backspace);
            editor.KeyBindings.Add<DeleteCommand>(ConsoleKey.Delete);
            editor.KeyBindings.Add<MoveHomeCommand>(ConsoleKey.Home);
            editor.KeyBindings.Add<MoveEndCommand>(ConsoleKey.End);
            editor.KeyBindings.Add<MoveLeftCommand>(ConsoleKey.LeftArrow);
            editor.KeyBindings.Add<MoveRightCommand>(ConsoleKey.RightArrow);
            editor.KeyBindings.Add<PreviousWordCommand>(ConsoleKey.LeftArrow, ConsoleModifiers.Control);
            editor.KeyBindings.Add<NextWordCommand>(ConsoleKey.RightArrow, ConsoleModifiers.Control);
            editor.KeyBindings.Add<SubmitCommand>(ConsoleKey.Enter);
            editor.KeyBindings.Add<NewLineCommand>(ConsoleKey.Enter, ConsoleModifiers.Shift);

            // Read a line (or many)
            var result = await editor.ReadLine(CancellationToken.None);

            // Write the buffer
            AnsiConsole.WriteLine();
            AnsiConsole.Render(new Panel(result.EscapeMarkup())
                .Header("[yellow]Program:[/]")
                .RoundedBorder());
        }
    }
}