using System;
using Spectre.Console;

namespace RadLine
{
    internal sealed class DefaultInputSource : IInputSource
    {
        private readonly IAnsiConsole _console;

        public bool ByPassProcessing => false;

        public DefaultInputSource(IAnsiConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public bool IsKeyAvailable()
        {
            return Console.KeyAvailable;
        }

        public ConsoleKeyInfo ReadKey()
        {
            if (!_console.Profile.Out.IsTerminal
                || !_console.Profile.Capabilities.Interactive)
            {
                throw new NotSupportedException("Only interactive terminals are supported as input source");
            }

            // TODO: Put terminal in raw mode
            return Console.ReadKey(true);
        }
    }
}
