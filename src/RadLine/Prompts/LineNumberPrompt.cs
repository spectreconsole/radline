using Spectre.Console;

namespace RadLine
{
    public sealed class LineNumberPrompt : ILineEditorPrompt
    {
        private readonly Style _style;

        public LineNumberPrompt(Style? style = null)
        {
            _style = style ?? new Style(foreground: Color.Yellow, background: Color.Blue);
        }

        public (Markup Markup, int Margin) GetPrompt(int line)
        {
            return (new Markup(line.ToString("D2"), _style), 1);
        }
    }
}
