using System;
using Spectre.Console;

namespace RadLine
{
    internal static class AnsiConsoleExtensions
    {
        public static IDisposable HideCursor(this IAnsiConsole console)
        {
            if (console is null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            return new CursorHider(console);
        }

        private sealed class CursorHider : IDisposable
        {
            private readonly IAnsiConsole _console;

            public CursorHider(IAnsiConsole console)
            {
                _console = console ?? throw new ArgumentNullException(nameof(console));
                _console.Cursor.Hide();
            }

            ~CursorHider()
            {
                throw new InvalidOperationException("CursorHider: Dispose was never called");
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                _console.Cursor.Show();
            }
        }
    }
}
