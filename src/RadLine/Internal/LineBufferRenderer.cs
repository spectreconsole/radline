using System;
using Spectre.Console;

namespace RadLine
{
    internal sealed class LineBufferRenderer
    {
        private readonly IAnsiConsole _console;
        private readonly AnsiRenderingStrategy _ansiRendererer;
        private readonly FallbackRenderingStrategy _fallbackRender;

        public LineBufferRenderer(IAnsiConsole console, IHighlighterAccessor accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _console = console ?? throw new ArgumentNullException(nameof(console));
            _ansiRendererer = new AnsiRenderingStrategy(console, accessor);
            _fallbackRender = new FallbackRenderingStrategy(console, accessor);
        }

        public void RenderLine(LineEditorState state, int? cursorPosition = null)
        {
            if (_console.Profile.Capabilities.Ansi)
            {
                _ansiRendererer.Render(state, cursorPosition);
            }
            else
            {
                _fallbackRender.Render(state, cursorPosition);
            }
        }
    }
}
