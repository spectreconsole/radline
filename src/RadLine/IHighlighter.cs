using Spectre.Console;

namespace RadLine
{
    public interface IHighlighter
    {
        Style? Highlight(string token);
    }
}
