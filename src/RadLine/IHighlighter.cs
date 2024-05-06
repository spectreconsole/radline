using Spectre.Console.Rendering;

namespace RadLine
{
    public interface IHighlighter
    {
        IRenderable BuildHighlightedText(string text);
    }
}
