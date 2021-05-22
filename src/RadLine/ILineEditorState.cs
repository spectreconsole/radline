namespace RadLine
{
    public interface ILineEditorState
    {
        public int LineIndex { get; }
        public int LineCount { get; }
        public bool IsFirstLine { get; }
        public bool IsLastLine { get; }
    }
}
