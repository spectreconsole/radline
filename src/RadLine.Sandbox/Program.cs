using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace RadLine.Sandbox
{
    public static class Program
    {
        public static async Task Main()
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            var editor = new LineEditor()
            {
                MultiLine = true,
                Text = "HELLO ABC WORLD DEF GHIJKLMN 🥰 PATRIK WAS HERE",
                Prompt = new LineNumberPrompt(),
                Completion = new TestCompletion(),
                Highlighter = new WordHighlighter()
                    .AddWord("git", new Style(foreground: Color.Yellow))
                    .AddWord("code", new Style(foreground: Color.Yellow))
                    .AddWord("vim", new Style(foreground: Color.Yellow))
                    .AddWord("init", new Style(foreground: Color.Blue))
                    .AddWord("push", new Style(foreground: Color.Red))
                    .AddWord("commit", new Style(foreground: Color.Blue))
                    .AddWord("rebase", new Style(foreground: Color.Red))
                    .AddWord("Hello", new Style(foreground: Color.Blue))
                    .AddWord("Goodbye", new Style(foreground: Color.Green))
                    .AddWord("World", new Style(foreground: Color.Yellow))
                    .AddWord("Syntax", new Style(decoration: Decoration.Strikethrough))
                    .AddWord("Highlighting", new Style(decoration: Decoration.SlowBlink)),
            };

            // Add custom commands
            editor.KeyBindings.Add<PrependSmiley>(ConsoleKey.I, ConsoleModifiers.Control);

            // Read a line
            var result = await editor.ReadLine(CancellationToken.None);

            // Write the buffer
            AnsiConsole.WriteLine();
            AnsiConsole.Render(new Panel(result.EscapeMarkup())
                .Header("[yellow]Commit details:[/]")
                .RoundedBorder());
        }
    }

    public sealed class PrependSmiley : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Execute(new PreviousWordCommand());
            context.Buffer.Insert(":-)");
        }
    }

    public sealed class TestCompletion : ITextCompletion
    {
        public IEnumerable<string> GetCompletions(string context, string word, string suffix)
        {
            if (string.IsNullOrWhiteSpace(context))
            {
                return new[] { "git", "code", "vim" };
            }

            if (context.Equals("git ", StringComparison.Ordinal))
            {
                return new[] { "init", "initialize", "push", "commit", "rebase" };
            }

            return null;
        }
    }
}