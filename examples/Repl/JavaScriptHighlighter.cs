using Spectre.Console;

namespace RadLine.Examples
{
    public sealed class JavaScriptHighlighter : IHighlighter
    {
        private readonly IHighlighter _highlighter;

        private static string[] _keywords = new[]
        {
            "await", "break", "case", "catch", "class",
            "const", "continue", "debugger", "default",
            "delete", "do", "else", "enum", "export",
            "extends", "false", "finally", "for", "function",
            "if", "implements", "import", "in", "instanceof",
            "interface", "let", "new", "null", "package",
            "private", "protected", "public", "return",
            "super", "switch", "static", "this", "throw",
            "try", "true", "typeof", "var", "void", "while",
            "with", "yield"
        };

        public JavaScriptHighlighter()
        {
            _highlighter = CreateHighlighter();
        }

        private static WordHighlighter CreateHighlighter()
        {
            var highlighter = new WordHighlighter();
            foreach (var keyword in _keywords)
            {
                highlighter.AddWord(keyword, new Style(foreground: Color.Blue));
            }

            highlighter.AddWord("{", new Style(foreground: Color.Grey));
            highlighter.AddWord("}", new Style(foreground: Color.Grey));
            highlighter.AddWord("(", new Style(foreground: Color.Grey));
            highlighter.AddWord(")", new Style(foreground: Color.Grey));
            highlighter.AddWord(";", new Style(foreground: Color.Grey));

            return highlighter;
        }

        public Style Highlight(string token)
        {
            return _highlighter.Highlight(token);
        }
    }
}