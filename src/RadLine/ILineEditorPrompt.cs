using Spectre.Console;

namespace RadLine
{
    public interface ILineEditorPrompt
    {
        (Markup Markup, int Margin) GetPrompt(int line);
    }
}
