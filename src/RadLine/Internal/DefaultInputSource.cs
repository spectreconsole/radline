using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace RadLine
{
    internal sealed class DefaultInputSource : IInputSource
    {
        private readonly IAnsiConsole _console;

        public DefaultInputSource(IAnsiConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task<ConsoleKeyInfo?> ReadKey(CancellationToken cancellationToken)
        {
            if (!_console.Profile.Out.IsTerminal
                || !_console.Profile.Capabilities.Interactive)
            {
                throw new NotSupportedException("Only interactive terminals are supported as input source");
            }

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (Console.KeyAvailable)
                {
                    break;
                }

                await Task.Delay(5, cancellationToken).ConfigureAwait(false);
            }

            return Console.ReadKey(true);
        }
    }
}
